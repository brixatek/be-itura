# ITURA — DevOps & Infrastructure

**Document Version:** 1.0  
**Owner:** DevOps / Platform Engineering  
**Last Updated:** May 2026  
**Stack:** Azure · Kubernetes · Docker · Terraform · Azure DevOps

---

## Table of Contents

1. [Git Branching Strategy](#1-git-branching-strategy)
2. [CI/CD Pipeline](#2-cicd-pipeline)
3. [Infrastructure as Code (Terraform)](#3-infrastructure-as-code-terraform)
4. [Containerization Strategy](#4-containerization-strategy)
5. [Kubernetes Deployment](#5-kubernetes-deployment)
6. [Scaling Strategy](#6-scaling-strategy)
7. [Secrets Management](#7-secrets-management)
8. [Monitoring & Observability](#8-monitoring--observability)
9. [Backup Strategy](#9-backup-strategy)
10. [Environments](#10-environments)

---

## 1. Git Branching Strategy

### Branch Model: GitHub Flow + Release Branches

```
main                    ← Production-ready code. Protected. Never push directly.
  └── release/v1.x      ← Release stabilization branch
  └── develop           ← Integration branch (auto-deploys to staging)
        ├── feature/BE-AUTH-001-email-registration
        ├── feature/FE-DASH-001-main-dashboard
        ├── bugfix/fix-mood-streak-calculation
        ├── hotfix/critical-payment-webhook-error  ← branches from main
        └── chore/update-dependencies
```

### Branch Naming Convention

```
feature/{ticket-id}-{short-description}   → feature/BE-AUTH-001-email-registration
bugfix/{ticket-id}-{short-description}    → bugfix/BE-MOOD-003-streak-reset-bug
hotfix/{ticket-id}-{short-description}    → hotfix/BE-PAY-007-double-charge-fix
chore/{description}                       → chore/upgrade-dotnet-8-packages
docs/{description}                        → docs/update-api-design
```

### Branch Protection Rules (main & develop)

- Require pull request before merging
- Require at least 1 approver (2 for main)
- Require all CI checks to pass
- Require linear history (squash or rebase merge)
- No direct push allowed
- Require branches to be up to date before merging

### Commit Message Convention

```
{type}({scope}): {short description}

Types: feat | fix | docs | chore | refactor | test | perf | ci | hotfix

Examples:
  feat(auth): add Google OAuth 2.0 login
  fix(mood): correct streak reset when using freeze
  hotfix(payment): prevent duplicate charge on webhook retry
  test(booking): add integration tests for booking saga
  ci: add SAST scan to CI pipeline
```

---

## 2. CI/CD Pipeline

### Pipeline Overview

```
Developer pushes to feature branch
    │
    ▼
┌─────────────────────────────────────────────────────┐
│              CI PIPELINE (Azure DevOps)             │
│                                                     │
│  Stage 1: Build & Test                             │
│    ├── Restore packages                             │
│    ├── Build solution                               │
│    ├── Run unit tests (xUnit)                       │
│    ├── Run integration tests                        │
│    ├── Code coverage report (≥80% gate)             │
│    ├── SAST scan (SonarQube / GitHub CodeQL)        │
│    └── Dependency vulnerability scan (Snyk)         │
│                                                     │
│  Stage 2: Container Build                           │
│    ├── Build Docker images (per service)            │
│    ├── Tag with commit SHA + branch name            │
│    ├── Container image scan (Trivy)                 │
│    └── Push to Azure Container Registry (ACR)      │
│                                                     │
│  Stage 3: Deploy to Staging (on develop merge)      │
│    ├── Helm chart update (new image tag)            │
│    ├── kubectl apply (Kubernetes rolling deploy)    │
│    ├── Smoke tests (critical path E2E)              │
│    └── Notify team on Slack                         │
│                                                     │
│  Stage 4: Deploy to Production (on release tag)     │
│    ├── Manual approval gate (Engineering Lead)      │
│    ├── Blue-green deployment switch                 │
│    ├── Health check verification                    │
│    ├── Run E2E test suite against production        │
│    └── Notify team on Slack                         │
└─────────────────────────────────────────────────────┘
```

### Azure DevOps Pipeline YAML

```yaml
# azure-pipelines.yml (root of repository)
trigger:
  branches:
    include:
      - main
      - develop
      - release/*
  paths:
    exclude:
      - docs/*
      - '*.md'

variables:
  dockerRegistryServiceConnection: 'acr-itura-prod'
  imageRepository: 'itura'
  containerRegistry: 'ituracr.azurecr.io'
  tag: '$(Build.SourceVersion)'
  vmImageName: 'ubuntu-latest'

stages:
  - stage: BuildAndTest
    displayName: 'Build and Test'
    jobs:
      - job: Build
        pool:
          vmImage: $(vmImageName)
        steps:
          - task: UseDotNet@2
            inputs:
              version: '8.0.x'

          - script: dotnet restore src/Itura.sln
            displayName: 'Restore Packages'

          - script: dotnet build src/Itura.sln --no-restore --configuration Release
            displayName: 'Build Solution'

          - script: |
              dotnet test src/Itura.sln \
                --no-build \
                --configuration Release \
                --collect:"XPlat Code Coverage" \
                --results-directory $(Agent.TempDirectory)/coverage
            displayName: 'Run Tests'

          - task: PublishCodeCoverageResults@1
            inputs:
              codeCoverageTool: 'cobertura'
              summaryFileLocation: '$(Agent.TempDirectory)/coverage/**/coverage.cobertura.xml'

          - task: SonarQubePrepare@5
            inputs:
              SonarQube: 'SonarQube-Service-Connection'
              scannerMode: 'MSBuild'
              projectKey: 'itura-backend'

          - task: SonarQubeAnalyze@5
          - task: SonarQubePublish@5
            inputs:
              pollingTimeoutSec: '300'

  - stage: ContainerBuild
    displayName: 'Build Containers'
    dependsOn: BuildAndTest
    condition: succeeded()
    jobs:
      - job: BuildImages
        steps:
          - task: Docker@2
            displayName: 'Build auth-service'
            inputs:
              containerRegistry: $(dockerRegistryServiceConnection)
              repository: 'itura/auth-service'
              command: 'buildAndPush'
              Dockerfile: 'src/Itura.Services/Auth/Dockerfile'
              tags: |
                $(tag)
                $(Build.SourceBranchName)-latest

          # Repeat for each service...

          - task: AquaSecurityTrivy@1
            displayName: 'Scan Container Images'
            inputs:
              image: '$(containerRegistry)/itura/auth-service:$(tag)'
              exitCode: '1'
              severity: 'CRITICAL,HIGH'

  - stage: DeployStaging
    displayName: 'Deploy to Staging'
    dependsOn: ContainerBuild
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
    jobs:
      - deployment: DeployToStaging
        environment: 'itura-staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: HelmDeploy@0
                  inputs:
                    connectionType: 'Kubernetes Service Connection'
                    kubernetesServiceConnection: 'aks-staging'
                    namespace: 'itura-staging'
                    command: 'upgrade'
                    chartType: 'FilePath'
                    chartPath: 'infrastructure/kubernetes/helm/itura'
                    releaseName: 'itura-staging'
                    overrideValues: 'image.tag=$(tag)'

                - script: |
                    # Run smoke tests
                    curl -f https://api.staging.itura.app/api/v1/health
                  displayName: 'Smoke Test'

  - stage: DeployProduction
    displayName: 'Deploy to Production'
    dependsOn: DeployStaging
    condition: and(succeeded(), startsWith(variables['Build.SourceBranch'], 'refs/tags/v'))
    jobs:
      - deployment: DeployToProduction
        environment: 'itura-production'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: ManualIntervention@8
                  displayName: 'Approval Gate'
                  inputs:
                    instructions: 'Review staging deployment. Approve to deploy to production.'
                    onTimeout: 'reject'
                    timeout: '60'

                - task: HelmDeploy@0
                  inputs:
                    kubernetesServiceConnection: 'aks-production'
                    namespace: 'itura-prod'
                    command: 'upgrade'
                    chartPath: 'infrastructure/kubernetes/helm/itura'
                    releaseName: 'itura-prod'
                    overrideValues: 'image.tag=$(tag)'
```

---

## 3. Infrastructure as Code (Terraform)

### Directory Structure

```
infrastructure/terraform/
├── environments/
│   ├── staging/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   └── terraform.tfvars
│   └── production/
│       ├── main.tf
│       ├── variables.tf
│       └── terraform.tfvars
│
├── modules/
│   ├── aks/              # Azure Kubernetes Service
│   ├── postgresql/       # Azure PostgreSQL Flexible Server
│   ├── redis/            # Azure Cache for Redis
│   ├── storage/          # Azure Blob Storage
│   ├── keyvault/         # Azure Key Vault
│   ├── monitoring/       # Log Analytics + App Insights
│   ├── frontdoor/        # Azure Front Door + WAF
│   └── networking/       # VNet, subnets, NSGs
│
└── shared/
    ├── providers.tf
    └── backend.tf        # Azure Blob backend for state
```

### Core Infrastructure (Production)

```hcl
# environments/production/main.tf

module "networking" {
  source              = "../../modules/networking"
  resource_group_name = "rg-itura-prod"
  location            = "westus3"
  vnet_address_space  = ["10.0.0.0/8"]
  aks_subnet          = "10.1.0.0/16"
  db_subnet           = "10.2.0.0/24"
  redis_subnet        = "10.3.0.0/24"
}

module "aks" {
  source              = "../../modules/aks"
  resource_group_name = "rg-itura-prod"
  location            = "westus3"
  cluster_name        = "aks-itura-prod"
  kubernetes_version  = "1.29"
  subnet_id           = module.networking.aks_subnet_id

  system_node_pool = {
    node_count      = 3
    vm_size         = "Standard_D4s_v3"
    os_disk_size_gb = 100
    min_count       = 3
    max_count       = 10
  }

  user_node_pool = {
    name            = "workload"
    node_count      = 3
    vm_size         = "Standard_D8s_v3"
    min_count       = 3
    max_count       = 30
    taints          = ["workload=true:NoSchedule"]
  }

  enable_azure_monitor    = true
  log_analytics_workspace = module.monitoring.workspace_id
}

module "postgresql" {
  source              = "../../modules/postgresql"
  resource_group_name = "rg-itura-prod"
  location            = "westus3"
  server_name         = "pg-itura-prod"
  sku_name            = "GP_Standard_D4s_v3"
  storage_mb          = 131072   # 128GB
  backup_retention_days = 30
  geo_redundant_backup  = true
  high_availability     = true
  subnet_id             = module.networking.db_subnet_id

  databases = [
    "itura_auth", "itura_users", "itura_coaches",
    "itura_bookings", "itura_payments", "itura_journal",
    "itura_mood", "itura_community", "itura_notifications",
    "itura_subscriptions", "itura_corporate", "itura_analytics"
  ]
}

module "redis" {
  source              = "../../modules/redis"
  resource_group_name = "rg-itura-prod"
  location            = "westus3"
  name                = "redis-itura-prod"
  capacity            = 2          # 13GB
  family              = "P"        # Premium
  sku_name            = "Premium"
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
  subnet_id           = module.networking.redis_subnet_id
}

module "key_vault" {
  source              = "../../modules/keyvault"
  resource_group_name = "rg-itura-prod"
  location            = "westus3"
  name                = "kv-itura-prod"
  sku_name            = "premium"
  enable_purge_protection = true
  soft_delete_retention_days = 90
}

module "storage" {
  source              = "../../modules/storage"
  resource_group_name = "rg-itura-prod"
  location            = "westus3"
  name                = "stituraprod"
  tier                = "Standard"
  replication_type    = "RAGRS"  # Read-access geo-redundant

  containers = ["avatars", "session-recordings", "coach-documents", "exports"]
}

module "front_door" {
  source              = "../../modules/frontdoor"
  resource_group_name = "rg-itura-prod"
  name                = "afd-itura-prod"
  sku_name            = "Premium_AzureFrontDoor"

  waf_policy = {
    mode = "Prevention"
    managed_rules = ["OWASP_3.2", "Microsoft_BotManagerRuleSet_1.0"]
    custom_rules = [
      # Block known bad IPs
      # Rate limiting by IP
    ]
  }
}
```

---

## 4. Containerization Strategy

### Dockerfile Pattern (Multi-stage Build)

```dockerfile
# src/Itura.Services/Auth/Dockerfile

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (leverage layer caching)
COPY ["Itura.Services.Auth/Itura.Services.Auth.csproj", "Itura.Services.Auth/"]
COPY ["Itura.Domain.Shared/Itura.Domain.Shared.csproj", "Itura.Domain.Shared/"]
RUN dotnet restore "Itura.Services.Auth/Itura.Services.Auth.csproj"

# Copy source
COPY . .
WORKDIR "/src/Itura.Services.Auth"
RUN dotnet publish "Itura.Services.Auth.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 2: Runtime (minimal image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Security: run as non-root
RUN groupadd -r appgroup && useradd -r -g appgroup appuser
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Itura.Services.Auth.dll"]
```

### Docker Compose (Local Development)

```yaml
# docker-compose.yml
version: '3.9'

services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: itura_dev
      POSTGRES_USER: itura
      POSTGRES_PASSWORD: localdev123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./scripts/init-db.sql:/docker-entrypoint-initdb.d/init.sql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    command: redis-server --requirepass localdev123

  mongodb:
    image: mongo:7
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: itura
      MONGO_INITDB_ROOT_PASSWORD: localdev123

  rabbitmq:
    image: rabbitmq:3.13-management-alpine
    ports:
      - "5672:5672"
      - "15672:15672"  # Management UI
    environment:
      RABBITMQ_DEFAULT_USER: itura
      RABBITMQ_DEFAULT_PASS: localdev123

  auth-service:
    build:
      context: ../src
      dockerfile: Itura.Services.Auth/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=itura_auth;Username=itura;Password=localdev123
      - Redis__ConnectionString=redis:6379,password=localdev123
      - RabbitMQ__Host=rabbitmq
    ports:
      - "5001:8080"
    depends_on:
      - postgres
      - redis
      - rabbitmq

  # ... other services

volumes:
  postgres_data:
```

---

## 5. Kubernetes Deployment

### Helm Chart Structure

```
infrastructure/kubernetes/helm/itura/
├── Chart.yaml
├── values.yaml                  # default values
├── values.staging.yaml          # staging overrides
├── values.production.yaml       # production overrides
└── templates/
    ├── _helpers.tpl
    ├── namespace.yaml
    ├── configmap.yaml
    ├── services/
    │   ├── auth-service/
    │   │   ├── deployment.yaml
    │   │   ├── service.yaml
    │   │   ├── hpa.yaml
    │   │   └── pdb.yaml           # PodDisruptionBudget
    │   └── ... (each service)
    ├── ingress.yaml
    └── monitoring/
        ├── servicemonitor.yaml
        └── prometheusrule.yaml
```

### Deployment Template

```yaml
# templates/services/auth-service/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ include "itura.fullname" . }}-auth
  labels:
    {{- include "itura.labels" . | nindent 4 }}
    app.kubernetes.io/component: auth-service
spec:
  replicas: {{ .Values.auth.replicaCount }}
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0          # zero-downtime deployments
  selector:
    matchLabels:
      app: auth-service
  template:
    metadata:
      labels:
        app: auth-service
      annotations:
        prometheus.io/scrape: "true"
        prometheus.io/port: "8080"
        prometheus.io/path: "/metrics"
    spec:
      serviceAccountName: itura-workload-identity
      securityContext:
        runAsNonRoot: true
        runAsUser: 1000
        fsGroup: 2000
      containers:
        - name: auth-service
          image: "{{ .Values.global.registry }}/itura/auth-service:{{ .Values.global.imageTag }}"
          imagePullPolicy: Always
          ports:
            - containerPort: 8080
              name: http
            - containerPort: 9090
              name: grpc
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: {{ .Values.environment }}
            - name: ConnectionStrings__DefaultConnection
              valueFrom:
                secretKeyRef:
                  name: auth-service-secrets
                  key: db-connection-string
          resources:
            requests:
              memory: "256Mi"
              cpu: "100m"
            limits:
              memory: "512Mi"
              cpu: "500m"
          livenessProbe:
            httpGet:
              path: /health/live
              port: 8080
            initialDelaySeconds: 30
            periodSeconds: 10
          readinessProbe:
            httpGet:
              path: /health/ready
              port: 8080
            initialDelaySeconds: 10
            periodSeconds: 5
          volumeMounts:
            - name: secrets-store
              mountPath: /mnt/secrets-store
              readOnly: true
      volumes:
        - name: secrets-store
          csi:
            driver: secrets-store.csi.k8s.io
            readOnly: true
            volumeAttributes:
              secretProviderClass: itura-azure-keyvault
```

### Horizontal Pod Autoscaler

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: auth-service-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: auth-service
  minReplicas: 2
  maxReplicas: 20
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70
    - type: Resource
      resource:
        name: memory
        target:
          type: Utilization
          averageUtilization: 80
  behavior:
    scaleUp:
      stabilizationWindowSeconds: 60
      policies:
        - type: Pods
          value: 4
          periodSeconds: 60
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
        - type: Pods
          value: 1
          periodSeconds: 120
```

### Pod Disruption Budget

```yaml
apiVersion: policy/v1
kind: PodDisruptionBudget
metadata:
  name: auth-service-pdb
spec:
  minAvailable: 1          # always keep at least 1 pod running
  selector:
    matchLabels:
      app: auth-service
```

---

## 6. Scaling Strategy

### Auto-Scaling Triggers

| Component | Scale-Up Trigger | Scale-Down Trigger | Min | Max |
|---|---|---|---|---|
| API Gateway | CPU > 70% | CPU < 30% for 5min | 2 | 10 |
| auth-service | CPU > 70% | CPU < 30% | 2 | 10 |
| booking-service | CPU > 70% | CPU < 30% | 2 | 15 |
| ai-service | CPU > 60% or queue depth > 100 | CPU < 20% | 2 | 20 |
| notification-service | Queue depth > 500 | Queue depth < 50 | 2 | 10 |
| community-service | CPU > 70% | CPU < 30% | 2 | 10 |

### KEDA (Kubernetes Event-Driven Autoscaling)

For queue-based scaling (RabbitMQ consumers):

```yaml
apiVersion: keda.sh/v1alpha1
kind: ScaledObject
metadata:
  name: notification-consumer-scaler
spec:
  scaleTargetRef:
    name: notification-service
  minReplicaCount: 2
  maxReplicaCount: 20
  triggers:
    - type: rabbitmq
      metadata:
        protocol: amqp
        queueName: notification.requests
        mode: QueueLength
        value: "50"              # 1 pod per 50 messages in queue
      authenticationRef:
        name: rabbitmq-trigger-auth
```

### Database Connection Pooling (PgBouncer)

```yaml
# PgBouncer as sidecar or standalone deployment
# Pool mode: transaction (optimal for microservices)
# Max client connections: 1000
# Server pool size: 25 per database
# This allows 1000 concurrent app connections with only 25 actual DB connections
```

---

## 7. Secrets Management

### Azure Key Vault Integration

**No secrets in environment variables, ConfigMaps, or source code.**

```yaml
# SecretProviderClass (Secrets Store CSI Driver)
apiVersion: secrets-store.csi.x-k8s.io/v1
kind: SecretProviderClass
metadata:
  name: itura-azure-keyvault
spec:
  provider: azure
  parameters:
    usePodIdentity: "false"
    useVMManagedIdentity: "true"
    userAssignedIdentityID: ${MANAGED_IDENTITY_CLIENT_ID}
    keyvaultName: kv-itura-prod
    cloudName: AzurePublicCloud
    objects: |
      array:
        - |
          objectName: auth-db-connection-string
          objectType: secret
          objectVersion: ""
        - |
          objectName: jwt-private-key
          objectType: secret
        - |
          objectName: paystack-secret-key
          objectType: secret
        - |
          objectName: stripe-secret-key
          objectType: secret
        - |
          objectName: azure-openai-api-key
          objectType: secret
        - |
          objectName: sendgrid-api-key
          objectType: secret
  secretObjects:
    - secretName: auth-service-secrets
      type: Opaque
      data:
        - objectName: auth-db-connection-string
          key: db-connection-string
        - objectName: jwt-private-key
          key: jwt-private-key
```

### Secret Rotation

| Secret | Rotation Frequency | Method |
|---|---|---|
| JWT signing key | Quarterly | Azure Key Vault rotation + reload |
| Database passwords | 6 months | Azure PostgreSQL + Key Vault |
| Paystack secret | On compromise | Manual + immediate |
| OpenAI API key | 6 months | Manual + Key Vault update |
| Internal service mTLS certs | 90 days | cert-manager auto-renewal |

---

## 8. Monitoring & Observability

### Grafana Dashboards

**Dashboard 1 — Platform Overview:**
- MAU / DAU (real-time)
- API request rate by service
- Error rate (5xx) by service
- P95 / P99 response times
- Active SignalR connections
- RabbitMQ queue depths
- Redis hit rate

**Dashboard 2 — Service Health:**
- Pod status by service (running/pending/failed)
- Pod restarts (last 24h)
- CPU/Memory utilization by pod
- HPA current vs desired replica count
- Node resource utilization

**Dashboard 3 — Business Metrics:**
- New registrations per hour
- Mood check-ins per hour
- Bookings per hour
- Revenue (today vs yesterday)
- AI companion messages per hour
- Community posts per hour

### Prometheus Alerting Rules

```yaml
# infrastructure/kubernetes/monitoring/alerts.yaml
groups:
  - name: itura-alerts
    rules:
      - alert: HighErrorRate
        expr: |
          sum(rate(http_requests_total{status=~"5.."}[5m])) /
          sum(rate(http_requests_total[5m])) > 0.01
        for: 5m
        labels:
          severity: critical
          team: engineering
        annotations:
          summary: "High API error rate detected ({{ $value | humanizePercentage }})"
          runbook: "https://wiki.itura.app/runbooks/high-error-rate"

      - alert: HighP95Latency
        expr: |
          histogram_quantile(0.95, sum(rate(http_request_duration_seconds_bucket[5m])) by (le, service)) > 0.5
        for: 5m
        labels:
          severity: warning

      - alert: PodCrashLoop
        expr: kube_pod_container_status_restarts_total > 3
        for: 5m
        labels:
          severity: critical

      - alert: CrisisEventDetected
        expr: increase(itura_crisis_events_total[5m]) > 0
        labels:
          severity: critical
          escalate: clinical-team

      - alert: PaymentFailureSpike
        expr: |
          sum(rate(itura_payment_failures_total[15m])) /
          sum(rate(itura_payment_attempts_total[15m])) > 0.05
        labels:
          severity: critical
```

### Distributed Tracing

```csharp
// OpenTelemetry configuration in each service
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddRedisInstrumentation()
        .AddSource("Itura.*")
        .AddOtlpExporter(opts => opts.Endpoint = new Uri(appInsightsEndpoint))
    )
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter()
    );
```

---

## 9. Backup Strategy

### Database Backups

```
PostgreSQL (Azure managed):
├── Continuous WAL (Write-Ahead Log) streaming
│   └── RPO: ~30 seconds
├── Automated daily full backup
│   └── Stored in Azure Blob (GRS)
├── Point-in-time restore: up to 30 days back
└── Geo-redundant backup: secondary region
    └── RTO for region failure: < 30 minutes

MongoDB (Azure Cosmos DB for MongoDB or self-managed):
├── Azure Backup integration: every 4 hours
└── Retention: 30 days

Redis:
├── Azure Cache for Redis Premium: AOF persistence
│   └── Append-only file every 15 minutes
└── RDB snapshots: every hour
```

### Application Data Backup

```
Azure Blob Storage (avatars, recordings, documents):
├── GRS (Geo-Redundant Storage): automatic replication to secondary region
├── Soft delete enabled: 30-day recovery window
└── Versioning enabled: recover overwritten objects
```

### Backup Verification

- Monthly restore drill: restore staging from production backup
- Restore time measurement documented
- Backup integrity check via checksum validation

---

## 10. Environments

### Environment Configuration

| Environment | Purpose | Cluster | Database | URL |
|---|---|---|---|---|
| Local | Developer laptop | Docker Compose | Local Docker containers | localhost |
| Development | Shared dev integration | Minimal AKS | Shared PostgreSQL (dev) | api.dev.itura.app |
| Staging | Pre-production validation | AKS (2 nodes) | Dedicated PostgreSQL | api.staging.itura.app |
| Production | Live | AKS (10+ nodes, multi-AZ) | Azure PostgreSQL HA | api.itura.app |
| DR (Disaster Recovery) | Standby | AKS (minimal, activates on DR) | PostgreSQL read replica promoted | api.dr.itura.app |

### Environment Promotion

```
Local → Development (automatic on PR merge to develop)
Development → Staging (automatic on develop CI pass)
Staging → Production (manual approval + tag v*.*.*)
Production → DR (infrastructure mirroring, not separate deployment)
```

### Feature Flags

Feature flags managed in database (admin-configurable without deployment):

```sql
-- feature_flags table (in admin service DB)
CREATE TABLE feature_flags (
    key         VARCHAR(100) PRIMARY KEY,
    enabled     BOOLEAN NOT NULL DEFAULT FALSE,
    tiers       VARCHAR(20)[],         -- NULL = all tiers
    percentage  INT DEFAULT 100,       -- rollout percentage
    description TEXT,
    updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Examples:
INSERT INTO feature_flags VALUES ('couples_wellness', FALSE, '{"Premium","Executive"}', 100);
INSERT INTO feature_flags VALUES ('ai_voice_companion', FALSE, NULL, 5); -- 5% beta rollout
INSERT INTO feature_flags VALUES ('community_challenges', TRUE, NULL, 100);
```

---

*End of DevOps & Infrastructure Document*  
*Next: [SECURITY.md](./SECURITY.md)*
