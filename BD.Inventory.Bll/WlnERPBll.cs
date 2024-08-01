using BD.Inventory.Dal;
using BD.Inventory.Entities;
using System;
using System.Collections.Generic;

namespace BD.Inventory.Bll
{
    public class WlnERPBll
    {
        private static readonly Lazy<WlnERPBll> _instance = new Lazy<WlnERPBll>(() => new WlnERPBll());

        public static WlnERPBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly WlnERPDal DalInstance;

        private WlnERPBll()
        {
            DalInstance = WlnERPDal.Instance;
        }

        /// <summary>
        /// 批量插入商品信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void InsertGoodsInfo(List<Goods> list, InsertGoodsResult result)
        {
            foreach (var item in list)
            {
                // 假设每个商品对象的specs属性都是有效的
                if (item.specs != null)
                {
                    // 将goods_code赋值给所有规格对象，避免在循环中重复访问属性
                    var goodsCode = item.goods_code;
                    foreach (var spec in item.specs)
                    {
                        spec.goods_code = goodsCode;
                    }
                }
            }
            DalInstance.InsertGoodsInfo(list, result);

        }

        /// <summary>
        /// 批量插入盘点单和仓库信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void InsertCheckBillInfo(List<InvCheckBillHead> list, InsertCheckBillResult result)
        {
            foreach (var item in list)
            {
                if (item.details != null)
                {
                    var billCode = item.bill_code;
                    foreach (var detail in item.details)
                    {
                        detail.bill_code = billCode;
                    }
                }
            }

            DalInstance.InsertCheckBillInfo(list, result);
        }

    }
}
