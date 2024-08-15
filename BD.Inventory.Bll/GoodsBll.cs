using BD.Inventory.Dal;
using BD.Inventory.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Bll
{
    public class GoodsBll
    {
        private static readonly Lazy<GoodsBll> _instance = new Lazy<GoodsBll>(() => new GoodsBll());

        public static GoodsBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly GoodsDal DalInstance;

        private GoodsBll()
        {
            DalInstance = GoodsDal.Instance;
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string tableName, string strWhere)
        {
            return DalInstance.IsExist(tableName, strWhere);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataTable GetPageList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return DalInstance.GetPageList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public DataTable GetDetail(string barcode, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            return DalInstance.GetDetail(barcode, pageSize, pageIndex, filedOrder, out recordCount);

        }

        /// <summary>
        /// 查询商品信息（生成盘点单用）
        /// </summary>
        /// <param name="goods_code">商品编码</param>
        /// <param name="sku_code">规格编码</param>
        /// <returns></returns>
        public GoodsDTO GetGoodsDTO(string bar_code)
        {
            return DalInstance.GetGoodsDTO(bar_code);
        }

        /// <summary>
        /// 条码绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int BindingCode(BindRFIDDTO model)
        {
            return DalInstance.BindingCode(model);
        }

        /// <summary>
        /// 条码绑定(批量绑定)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int BindingCodeBatch(BindRFIDDTO model)
        {
            return DalInstance.BindingCodeBatch(model);
        }

        /// <summary>
        /// 删除详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DelDetail(string id)
        {
            return DalInstance.DelDetail(id);
        }

    }
}
