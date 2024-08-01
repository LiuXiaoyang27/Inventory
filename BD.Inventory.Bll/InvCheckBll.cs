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
        /// 根据单号查询单头
        /// </summary>
        /// <param name="bill_code"></param>
        /// <returns></returns>
        public InvCheckDTO GetModelByBillCode(string bill_code)
        {
            return DalInstance.GetModelByBillCode(bill_code);
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
        public DataTable GetDetail(string bill_code, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            return DalInstance.GetDetail(bill_code, pageSize, pageIndex, filedOrder, out recordCount);

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

        // ============================= 以下为手持端方法 ==================================================

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
