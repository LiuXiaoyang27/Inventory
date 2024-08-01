namespace BD.Inventory.Entities
{
    public class InsertGoodsResult
    {
        public int goods_insert_count { get; set; }

        public int spec_insert_count { get; set; }
       
    }

    public class InsertCheckBillResult
    {       

        public int check_bill_head_insert_count { get; set; }

        public int check_bill_body_insert_count { get; set; }

        public int storage_insert_count { get; set; }
    }
}
