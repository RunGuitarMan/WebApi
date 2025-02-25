#!/usr/bin/env bash
# init-replica.sh

echo "=== [REPLICA INIT] Start custom configuration ==="

# Вариант 1: Если data уже пустая, можно попробовать pg_basebackup.
# Но имейте в виду, что после инициализации master уже что-то мог записать.
# В реальном production-сценарии лучше использовать docker-healthcheck
# и init контейнер до запуска основного.

# Пример (очень упрощённо):
cat <<EOF >> /var/lib/postgresql/data/postgresql.conf
listen_addresses = '*'
hot_standby = on
EOF

# Далее, если хотите действительно подтянуть данные с мастера до запуска:
# pg_basebackup -h webapi-postgres-master -D /var/lib/postgresql/data --wal-method=stream --username=replication_user --password

echo "=== [REPLICA INIT] Finished ==="