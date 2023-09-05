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
    url: "/login",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerInfo),
  });
// 重新获取学生信息接口
export const initReader = (readerId) =>
  requests({
    url: "/initreader",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId),
  });
// 重置密码
export const change_pwd = (
  readerId,
  oldPassword,
  newPassword,
  confirmNewPassword
) =>
  requests({
    url: "/change_pwd",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(
      readerId,
      oldPassword,
      newPassword,
      confirmNewPassword
    ),
  });
// 注销账户
export const logout_reader = (readerId) =>
  requests({
    url: "/logout_reader",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId),
  });
// 提交反馈
export const reader_report = (readerId, content) =>
  requests({
    url: "/reader_report",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId, content),
  });
// 评论区接口
export const initCommentsList = () =>
  requests({
    url: "/comments",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
  });
// 书籍接口
export const initBooksList = () =>
  requests({
    url: "/books",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
  });
// 添加评论接口
export const addComment = (dataObj) =>
  requests({
    url: "/addcomment",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(dataObj),
  });
// 删除评论接口
export const auditComment = (infoObj) =>
  requests({
    url: "/auditcomment",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(infoObj),
  });
// 删除预约记录接口
export const deleteReserve = (reserveObj) =>
  requests({
    url: "/cancelreserve",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(reserveObj),
  });
// 借书接口
export const addBorrow = (borrowObj) =>
  requests({
    url: "/addborrow",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(borrowObj),
  });

// 书名查找接口
export const searchBook = (bookNameObj) =>
  requests({
    url: "/searchbook",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(bookNameObj),
  });

// 读者请求借阅记录接口
export const initBorrows = (readerId) =>
  requests({
    url: "/borrows",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerId),
  });
// 添加预约记录接口
export const addReserve = (reserveObj) =>
  requests({
    url: "/addreserve",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(reserveObj),
  });
// 续借接口
export const continueBorrow = (infoObj) =>
  requests({
    url: "/continueborrow",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(infoObj),
  });
// 还书接口
export const returnBook = (infoObj) =>
  requests({
    url: "/returnbook",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(infoObj),
  });
// 查询预约接口
export const initReserve = (readerObj) =>
  requests({
    url: "/reserve",
    method: "post",
    headers: {
      "Content-Type": "application/json",
    },
    data: JSON.stringify(readerObj),
  });
