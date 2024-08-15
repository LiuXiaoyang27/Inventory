using BD.Inventory.Common;
using BD.Inventory.Dal;
using BD.Inventory.Entities;
using BD.Inventory.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Bll
{
    public class InvCheckBll
    {
        private static readonly Lazy<InvCheckBll> _instance = new Lazy<InvCheckBll>(() => new InvCheckBll());

        public static InvCheckBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly InvCheckDal DalInstance;

        private InvCheckBll()
        {
            DalInstance = InvCheckDal.Instance;
        }

        // ============================= 以下为PC端方法 ===================================================

        #region 生成盘点单号
        /// <summary>
        /// 生成盘点单号
        /// </summary>
        /// <returns></returns>
        public string InventoryNumberGenerator()
        {
            return DalInstance.InventoryNumberGenerator();
        }
        #endregion

        #region 创建盘点单
        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreateCheckBill(List<InvInfo> list, string username, ref string msg)
        {
            InvCheckBillHead model = ConvertToCheckBill(list, username, ref msg);
            if (model != null)
            {
                return DalInstance.CreateCheckBill(model);
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 将获取的库存数据转换成盘点单对象 TODO: 后续可能需要修改
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public InvCheckBillHead ConvertToCheckBill(List<InvInfo> list, string username, ref string msg)
        {
            if (list == null || list.Count == 0)
            {
                msg = "未获取到库存信息";
                return null;
            }
            DateTime today = DateTime.Now;

            // 表头
            InvCheckBillHead model = new InvCheckBillHead
            {
                bill_code = InventoryNumberGenerator(),
                bill_creater = username,
                bill_date = today,
                create_time = today,
                remark = "系统生成单据",
                state = 2,
                storage_code = list[0].storage_code,
                storage_name = list[0].storage_name
            };
            // 表体
            List<InvCheckBillBody> details = new List<InvCheckBillBody>();
            InvCheckBillBody detail;
            GoodsBll goods_instance = GoodsBll.Instance;
            foreach (var inv_info in list)
            {
                // 如果该商品没有库存，则不添加到盘点表体中
                if (inv_info.quantity <= 0)
                {
                    continue;
                }
                GoodsDTO goods_dto = goods_instance.GetGoodsDTO(inv_info.bar_code);
                if (goods_dto == null)
                {
                    msg = $"查询不到商品信息(商品条码：{inv_info.bar_code})，请同步商品后重试";
                    return null;
                }
                detail = new InvCheckBillBody
                {
                    goods_code = inv_info.goods_code,
                    goods_name = goods_dto.goods_name,
                    bill_code = model.bill_code,
                    change_size = 0,
                    index = 0,
                    nums = 0,
                    price = (decimal?)goods_dto.sale_price,
                    quantity = 0,
                    quantity_start = (double?)inv_info.quantity,
                    remark = "系统根据库存生成单据",
                    spec_code = inv_info.sku_code,
                    spec_name = inv_info.spec_name,
                    total_money = inv_info.cost,
                    unit = goods_dto.unit_name,
                    bar_code = inv_info.bar_code
                };
                details.Add(detail);

            }
            model.details = details;

            return model;

        }

        #endregion

        /// <summary>
        /// 查询盘点单头（分页）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable SelInvCheckHead(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return DalInstance.SelInvCheckHead(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }



        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetDetail(string strWhere, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            return DalInstance.GetDetail(strWhere, pageSize, pageIndex, filedOrder, out recordCount);

        }

        /// <summary>
        /// 批量删除盘点单
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int DeleteBatch(List<string> bill_code_list, string userName)
        {
            return DalInstance.DeleteBatch(bill_code_list, userName);
        }

        /// <summary>
        /// 批量完成盘点单
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int Complete(List<string> bill_code_list)
        {
            return DalInstance.Complete(bill_code_list);
        }


        #region 添加增量盘点单相关逻辑 （旧逻辑 弃用）

        /// <summary>
        /// 查询盘点单头（分页）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable SelInvCheckHead1(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return DalInstance.SelInvCheckHead1(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }



        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="bill_code"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetDetail1(string bill_code, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            return DalInstance.GetDetail1(bill_code, pageSize, pageIndex, filedOrder, out recordCount);

        }

        /// <summary>
        /// 根据单号查询单头
        /// </summary>
        /// <param name="bill_code"></param>
        /// <returns></returns>
        public InvCheckDTO GetModelByBillCode(string bill_code)
        {
            return DalInstance.GetModelByBillCode(bill_code);
        }

        /// <summary>
        /// 获取同步日期
        /// </summary>
        /// <returns></returns>
        public SyncDate GetSyncDate()
        {
            return DalInstance.GetSyncDate();
        }

        /// <summary>
        /// 设置同步时间
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool SetSyncTime(SyncDate model)
        {
            return DalInstance.SetSyncTime(model);
        }

        /// <summary>
        /// 修改同步时间
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool UpdateSyncTime(SyncDate model)
        {
            return DalInstance.UpdateSyncTime(model);
        }

        #endregion

        // ============================= 以下为手持端方法 ==================================================

        /// <summary>
        /// 查询单号
        /// </summary>
        /// <returns></returns>
        public DataTable GetBillCode(string strWhere)
        {
            return DalInstance.GetBillCode(strWhere);
        }

        /// <summary>
        /// 查询仓库
        /// </summary>
        /// <returns></returns>
        public DataTable GetStorage(string strWhere)
        {
            return DalInstance.GetStorage(strWhere);
        }

        /// <summary>
        /// 通过单据编码查询盘点相关数据
        /// </summary>
        /// <param name="bill_code"></param>
        /// <returns></returns>
        public ChooseBillCodeDTO SelDataByBillCode(string bill_code, int pageIndex, int pageSize)
        {
            return DalInstance.SelDataByBillCode(bill_code, pageIndex, pageSize);
        }

        /// <summary>
        /// 盘点提交
        /// </summary>
        /// <param name="RFIDList"></param>
        /// <param name="bill_code"></param>
        /// <param name="isRepeat"></param>
        /// <returns></returns>
        public bool InvSubmit(HashSet<string> scannedRFIDsSet, string bill_code, int isRepeat)
        {
            return DalInstance.InvSubmit(scannedRFIDsSet, bill_code, isRepeat);
        }

    }
}
