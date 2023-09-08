// 对所有的api接口进行统一管理
import requests from "./request";

// 注册接口
export const register = (readerInfo) =>
  requests({
    url: "/UserApi/register",
    method: "post",
    headers: {
        "Content-Type": "application/json",
      },
     data: JSON.stringify(readerInfo),
  });
// 用户登录接口
export const login_reader = (readerInfo) =>
  requests({
      url: "/UserApi/login",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerInfo),
  });
// 管理员登录接口
export const login_admin = (adminInfo) =>
    requests({
        url: "/AdminApi/login",
        method: "post",
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify(adminInfo),
    });
// 重新获取学生信息接口
export const initReader = (readerId) =>
  requests({
      url: "/UserApi/initreader",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId),
  });
// 重置密码
export const change_pwd = (
    reader_id,
  oldPassword,
  newPassword,
  confirmNewPassword
) =>
  requests({
      url: "/UserApi/updatepassword",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(
        reader_id,
      oldPassword,
      newPassword,
      confirmNewPassword
    ),
  });
// 注销账户
export const logout_reader = (readerId) =>
  requests({
      url: "/UserApi/logout",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId),
  });
// 提交反馈
export const reader_report = (reportInfo) =>
  requests({
      url: "/UserApi/readerreport",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
      data: JSON.stringify(reportInfo),
  });
// 评论区接口
export const initCommentsList = (BOOKobj) =>
  requests({
      url: "/UserApi/initcommentslist",
    method: "post",
    headers: {
      "Content-Type": "application/json",
      },
      data: JSON.stringify(BOOKobj),
  });
// 添加评论接口
export const addComment = (dataObj) =>
  requests({
      url: "/UserApi/addreview",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(dataObj),
  });
// 删除评论接口
export const auditComment = (infoObj) =>
  requests({
      url: "/UserApi/deletereview",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(infoObj),
  });
// 删除预约记录接口
export const deleteReserve = (reserveObj) =>
  requests({
      url: "/UserApi/deletereserve",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(reserveObj),
  });
// 借书接口
export const addBorrow = (borrowObj) =>
  requests({
      url: "/UserApi/borrowbook",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(borrowObj),
  });

// 书名查找接口
export const searchBook = (bookNameObj) =>
  requests({
      url: "/UserApi/searchBook",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(bookNameObj),
  });

// 读者请求借阅记录接口
export const initBorrows = (readerId) =>
  requests({
      url: "/UserApi/initborrows",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId),
  });
// 添加预约记录接口
export const addReserve = (reserveObj) =>
  requests({
      url: "/UserApi/reservebook",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(reserveObj),
  });
// 续借接口
export const continueBorrow = (infoObj) =>
  requests({
      url: "/UserApi/continueborrow",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(infoObj),
  });
// 还书接口
export const returnBook = (infoObj) =>
  requests({
      url: "/UserApi/returnbook",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(infoObj),
  });
// 查询预约接口
export const initReserve = (readerId) =>
  requests({
      url: "/UserApi/initreserve",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
      data: JSON.stringify(readerId),
  });
//预约超时接口
export const ReserveOvertime = (infoObj) =>
    requests({
        url: "/UserApi/reserveovertime",
        method: "post",
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify(infoObj),
    });
// 借书超时接口
export const BorrowOvertime = (infoObj) =>
    requests({
        url: "/UserApi/borrowovertime",
        method: "post",
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify(infoObj),
    });

// 重新获取管理员信息接口
export const initAdmin = (adminId) =>
    requests({
        url: "/AdminApi/initadmin",
        method: "post",
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify(adminId),
    });

// 重新获取所有用户信息接口
export const initReaderList = () =>
    requests({
        url: "/AdminApi/initreaderlist",
        method: "post",
    });
// 管理员重置密码
export const change_pwd_admin = (
    admin_id,
    oldPassword,
    newPassword,
    confirmNewPassword
) =>
    requests({
        url: "/AdminApi/updatepassword",
        method: "post",
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify(
            admin_id,
            oldPassword,
            newPassword,
            confirmNewPassword
        ),
    });
// 管理员修改用户违规次数接口
export const updateCount = (Obj) =>
    requests({
        url: "/AdminApi/updatecount",
        method: "post",
        headers: {
            "Content-Type": "application/json",
        },
        data: JSON.stringify(Obj),
    });
// 管理员查看反馈接口
export const initReportList = () =>
    requests({
        url: "/AdminApi/initreportlist",
        method: "post",
    });
// 管理员查看预约接口
export const initReservelist = () =>
    requests({
        url: "/AdminApi/initreservelist",
        method: "post",
    });
// 管理员查看借阅接口
export const initBorrowslist = () =>
    requests({
        url: "/AdminApi/initborrowslist",
        method: "post",
    });
// 供应商接口
export const initSupplierList = () =>
    requests({
        url: "/AdminApi/initsupplierlist",
        method: "post",
    });



//// 修改书接口
//export const searchBook = (bookNameObj) =>
//    requests({
//        url: "/AdminApi/searchBook",
//        method: "post",
//        headers: {
//            "Content-Type": "application/json",
//        },
//        data: JSON.stringify(bookNameObj),
//    });
//// 删除书接口
//export const searchBook = (bookNameObj) =>
//    requests({
//        url: "/AdminApi/searchBook",
//        method: "post",
//        headers: {
//            "Content-Type": "application/json",
//        },
//        data: JSON.stringify(bookNameObj),
//    });