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
// 登录接口
export const login = (readerInfo) =>
  requests({
      url: "/UserApi/login",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerInfo),
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
export const initCommentsList = () =>
  requests({
      url: "/UserApi/comments",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
  });
  /*
// 书籍接口
export const initBooksList = () =>
  requests({
      url: "/UserApi/books",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
  });
  */
// 添加评论接口
export const addComment = (dataObj) =>
  requests({
      url: "/UserApi/addcomment",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(dataObj),
  });
// 删除评论接口
export const auditComment = (infoObj) =>
  requests({
      url: "/UserApi/auditcomment",
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
