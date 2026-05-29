#!/usr/bin/env bash
# Hetzner Cloud — one-time Swarm initialisation
# Run this on the MANAGER node after provisioning all 3 CX42 servers.
#
# Usage:
#   ssh root@<manager-ip> 'bash -s' < deploy/scripts/init-swarm.sh
#
# After this script:
#   1. Add worker nodes:  docker swarm join --token <token> <manager-ip>:2377
#   2. Copy stack files:  scp deploy/docker-stack.yml deploy@<manager-ip>:/opt/itura/
#   3. Create secrets:    see the "Secrets" section below
#   4. Deploy:            IMAGE_TAG=latest REPO=<ghcr-owner> docker stack deploy -c /opt/itura/docker-stack.yml itura

set -euo pipefail

DOMAIN="${DOMAIN:-itura.app}"
DEPLOY_USER="deploy"

echo "==> [1/8] System update"
apt-get update -qq && apt-get upgrade -y -qq

echo "==> [2/8] Install Docker"
curl -fsSL https://get.docker.com | sh
systemctl enable docker
systemctl start docker

echo "==> [3/8] Configure firewall (ufw)"
ufw default deny incoming
ufw default allow outgoing
ufw allow 22/tcp    # SSH
ufw allow 80/tcp    # HTTP (Traefik redirect)
ufw allow 443/tcp   # HTTPS
ufw allow 2377/tcp  # Swarm manager
ufw allow 7946/tcp  # Swarm node communication
ufw allow 7946/udp
ufw allow 4789/udp  # Overlay network
ufw --force enable

echo "==> [4/8] Create deploy user"
if ! id "$DEPLOY_USER" &>/dev/null; then
    useradd -m -s /bin/bash "$DEPLOY_USER"
    usermod -aG docker "$DEPLOY_USER"
fi
mkdir -p /home/$DEPLOY_USER/.ssh
# Copy authorized_keys from root (CI SSH key should already be in root's authorized_keys)
cp /root/.ssh/authorized_keys /home/$DEPLOY_USER/.ssh/authorized_keys 2>/dev/null || true
chown -R $DEPLOY_USER:$DEPLOY_USER /home/$DEPLOY_USER/.ssh
chmod 700 /home/$DEPLOY_USER/.ssh
chmod 600 /home/$DEPLOY_USER/.ssh/authorized_keys

echo "==> [5/8] Initialise Docker Swarm"
MANAGER_IP=$(hostname -I | awk '{print $1}')
docker swarm init --advertise-addr "$MANAGER_IP"

echo ""
echo ">>> Worker join token (run on each worker node):"
docker swarm join-token worker
echo ""

echo "==> [6/8] Create overlay networks"
docker network create --driver overlay --attachable traefik-public   || true
docker network create --driver overlay --attachable itura-overlay    || true

echo "==> [7/8] Create directory structure"
mkdir -p /opt/itura
mkdir -p /opt/monitoring
chown -R $DEPLOY_USER:$DEPLOY_USER /opt/itura
chown -R $DEPLOY_USER:$DEPLOY_USER /opt/monitoring

echo "==> [8/8] Label this node for monitoring workloads"
docker node update --label-add monitoring=true "$(docker node ls --format '{{.ID}}' | head -1)"

echo ""
echo "============================================================"
echo "  Swarm manager ready at: $MANAGER_IP"
echo "  Domain: $DOMAIN"
echo ""
echo "  Next steps:"
echo "  1. Join worker nodes using the token printed above"
echo "  2. Create Docker secrets (see below)"
echo "  3. Copy config files to /opt/monitoring/"
echo "  4. Deploy the stack"
echo "============================================================"
echo ""
echo "  Create secrets with:"
echo "    docker secret create itura_jwt_private_key      ./private.pem"
echo "    docker secret create itura_jwt_public_key       ./public.pem"
echo "    docker secret create itura_db_password          <(echo -n 'your-db-password')"
echo "    docker secret create itura_anthropic_api_key    <(echo -n 'sk-ant-...')"
echo "    docker secret create itura_paystack_secret_key  <(echo -n 'sk_live_...')"
echo "    docker secret create itura_smtp_password        <(echo -n 'your-smtp-password')"
echo "    docker secret create itura_apns_private_key     ./AuthKey_XXXXXXXXXX.p8"
echo "    docker secret create itura_firebase_credentials ./firebase-service-account.json"
echo "    docker secret create itura_storage_secret_key   <(echo -n 'your-storage-secret')"
echo ""
echo "  Deploy:"
echo "    export IMAGE_TAG=latest REPO=<your-ghcr-username> DOMAIN=$DOMAIN"
echo "    docker stack deploy --with-registry-auth -c /opt/itura/docker-stack.yml itura"
