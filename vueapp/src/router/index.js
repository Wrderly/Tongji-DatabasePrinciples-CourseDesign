import Vue from "vue";
import VueRouter from "vue-router";

Vue.use(VueRouter);

import LoginRegister from "@/pages/LoginRegister";
import Home from "@/pages/Home";
import Userhome from "@/pages/Home/Userhome";
import SearchBooks from "@/pages/Home/SearchBooks";
import ReaderBorrows from "@/pages/Home/ReaderBorrows";
import ReaderReserve from "@/pages/Home/ReaderReserve";
import Comment from "@/pages/Home/Comment";
import ReaderReport from "@/pages/Home/ReaderReport";
import UserInfor from "@/pages/Home/UserInfor";
import Person from "@/pages/Home/Person";
import Schedule from "@/pages/Home/Schedule";
import AdminReport from '@/pages/Home/AdminReport'


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
          path: "userhome",
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
          component: ReaderReserve,
        },
        {
          //     读者举报反馈
          path: "readerreport",
          component: ReaderReport,
        },
        {
          //     读者个人信息
            path: "userinfor",
            component: UserInfor,
          },
          {
              //     人员管理
              path: "person",
              component: Person,
          },
          {
              //     排班表
              path: "schedule",
              component: Schedule,
          },
          {
              //     管理员反馈
              path: "adminreport",
              component: AdminReport,
          }
      ],
    },
  ],
});
