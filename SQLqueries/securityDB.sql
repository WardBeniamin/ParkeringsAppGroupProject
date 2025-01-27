--CREATE SYMMETRIC KEY MyKey
--WITH ALGORITHM = AES_256
--ENCRYPTION BY PASSWORD = 'StrongPassword123';

-- Open the symmetric key
OPEN SYMMETRIC KEY MyKey
DECRYPTION BY PASSWORD = 'StrongPassword123';

-- Encrypt the Password column and store it in EncryptedPassword
UPDATE dbo.Users
SET EncryptedPassword = ENCRYPTBYKEY(KEY_GUID('MyKey'), CAST(Password AS VARCHAR(50)));

-- Close the symmetric key
CLOSE SYMMETRIC KEY MyKey;


-- Open the symmetric key
OPEN SYMMETRIC KEY MyKey
DECRYPTION BY PASSWORD = 'StrongPassword123';

-- Select decrypted data
SELECT UserId, FullName, Email,
       CAST(DECRYPTBYKEY(EncryptedPassword) AS VARCHAR(50)) AS DecryptedPassword
FROM dbo.Users;

-- Close the symmetric key
CLOSE SYMMETRIC KEY MyKey;