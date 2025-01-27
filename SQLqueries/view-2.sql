
CREATE VIEW ViewUserTransactionSummary AS
SELECT 
    U.UserId,
    U.FullName AS UserName,
    COUNT(R.TransactionId) AS TotalTransactions,
    SUM(R.Amount) AS TotalAmountPaid,
    COUNT(DISTINCT C.CarId) AS TotalCarsUsed
FROM 
    Users U
LEFT JOIN 
    Receipts R ON U.UserId = R.UserId
LEFT JOIN 
    Cars C ON R.CarId = C.CarId
GROUP BY 
    U.UserId, U.FullName;
