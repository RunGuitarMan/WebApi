#!/usr/bin/env bash
# init-master.sh

echo "=== [MASTER INIT] Start custom configuration for PostgreSQL ==="

# Допишем настройки репликации в главный postgresql.conf:
cat <<EOF >> /var/lib/postgresql/data/postgresql.conf
listen_addresses = '*'
wal_level = replica
synchronous_commit = on
synchronous_standby_names = 'replica'
max_wal_senders = 10
EOF

# Создадим пользователя репликации (если нужно отдельного):
psql --username "${POSTGRES_USER}" --dbname "${POSTGRES_DB}" <<-EOSQL
    CREATE ROLE replication_user WITH REPLICATION LOGIN ENCRYPTED PASSWORD 'example_password';
EOSQL

echo "=== [MASTER INIT] Finished ==="