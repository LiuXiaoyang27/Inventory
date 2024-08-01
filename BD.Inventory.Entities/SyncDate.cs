namespace BD.Inventory.Entities
{
    public partial class SyncDate
    {
        public SyncDate()
        {


        }

        public string ID { get; set; }

        /// <summary>
        /// Desc:年
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int Year { get; set; }

        /// <summary>
        /// Desc:月
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int Month { get; set; }

        /// <summary>
        /// Desc:日
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int Day { get; set; }

        /// <summary>
        /// Desc:时
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int Hour { get; set; }

        /// <summary>
        /// Desc:分
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int Minute { get; set; }

        /// <summary>
        /// Desc:秒
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int Second { get; set; }

    }
}
