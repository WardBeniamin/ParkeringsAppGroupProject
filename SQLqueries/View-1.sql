CREATE VIEW UserCarTotalAmount AS
SELECT 
    U.FullName,
    R.CarId,
    SUM(R.Amount) AS TotalAmount
FROM 
    Users U
JOIN 
    Receipts R
ON 
    U.UserId = R.UserId
GROUP BY 
    U.FullName, R.CarId;