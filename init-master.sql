-- Создаем пользователя для репликации
CREATE ROLE replication_user WITH REPLICATION LOGIN ENCRYPTED PASSWORD 'example_password';

-- Разрешаем доступ для репликации
-- В pg_hba.conf будет добавлено разрешение на подключение для репликации,
-- но это можно также сделать через параметры в контейнере (которые указаны в docker-compose)
-- Добавляем правило для репликации:
-- host    replication     all             0.0.0.0/0               md5

-- Можно также настроить дополнительные параметры, если нужно
-- Пример: Настройка для синхронной репликации
SELECT pg_create_physical_replication_slot('replica_slot');