using System;

namespace Abshire_Ed.Models
{
    public class SaleTransactionModel
    {
        public int PersonId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionTime { get; set; }
        public int SaleId { get; set; }
    }
}
