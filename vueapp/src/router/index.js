import Vue from "vue";
import VueRouter from "vue-router";

Vue.use(VueRouter);

import LoginRegister from "@/pages/LoginRegister";
import Home from "@/pages/Home";
import Userhome from "@/pages/Home/Userhome";
import SearchBooks from "@/pages/Home/SearchBooks";
import ReaderBorrows from "@/pages/Home/ReaderBorrows";
import ReaderRserve from "@/pages/Home/ReaderRserve";
import Comment from "@/pages/Home/Comment";
import ReaderReport from "@/pages/Home/ReaderReport";
import ReaderInfor from "@/pages/Home/ReaderInfor";

export default new VueRouter({
  routes: [
    {
      path: "*",
      redirect: "/LoginRegister",
    },
    {
      path: "/LoginRegister",
      component: LoginRegister,
    },
    {
      path: "/home",
      component: Home,
      children: [
        {
          path: "/",
          component: Userhome,
        },
        {
          //    主页介绍
          path: "Userhome",
          component: Userhome,
        },
        {
          //    书籍查询
          path: "search",
          component: SearchBooks,
        },
        {
          //    评论区记录
          path: "comment",
          component: Comment,
        },
        // 读者
        {
          //    读者借阅记录
          path: "readerborrows",
          component: ReaderBorrows,
        },
        {
          //    读者预约记录
          path: "readerreserve",
          component: ReaderRserve,
        },
        {
          //     读者举报反馈
          path: "readerreport",
          component: ReaderReport,
        },
        {
          //     读者个人信息
          path: "ReaderInfor",
          component: ReaderInfor,
        },
      ],
    },
  ],
});
