-- skapa en readonly user
CREATE LOGIN ReadOnlyUser WITH PASSWORD = 'ReadOnlyPass123';

CREATE USER ReadOnlyUser FOR LOGIN ReadOnlyUser;

EXEC sp_addrolemember 'db_datareader', 'ReadOnlyUser';

-- skapa en owner user

CREATE LOGIN OwnerUser WITH PASSWORD = 'ReadAllPass123';

CREATE USER OwnerUser FOR LOGIN OwnerUser;

EXEC sp_addrolemember 'db_owner', 'OwnerUser';

-- ge skrivbehörighet
GRANT SELECT ON Users TO ReadOnlyUser;

-- ge fullständig åtkomst
GRANT SELECT, INSERT, UPDATE, DELETE ON Users TO OwnerUser;