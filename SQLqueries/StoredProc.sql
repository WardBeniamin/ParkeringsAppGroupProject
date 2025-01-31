CREATE PROCEDURE GetUserReceiptSummary
    @UserId INT
AS
BEGIN
    SELECT 
        COALESCE(SUM(Amount), 0) AS TotalAmount,
        COUNT(*) AS ReceiptCount
    FROM Receipts
    WHERE UserId = @UserId;
END;

EXEC GetUserReceiptSummary @UserId = 1;
