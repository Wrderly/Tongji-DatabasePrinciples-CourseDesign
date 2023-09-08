import Vue from "vue";
import VueRouter from "vue-router";

Vue.use(VueRouter);

import LoginRegister from "../pages/LoginRegister/index.vue";
import Home from "../pages/Home/index.vue";
import Userhome from "../pages/Home/Userhome/index.vue";
import SearchBooks from "../pages/Home/SearchBooks/index.vue";
import ReaderBorrows from "../pages/Home/ReaderBorrows/index.vue";
import ReaderReserve from "../pages/Home/ReaderReserve/index.vue";
import Comment from "../pages/Home/Comment/index.vue";
import ReaderReport from "../pages/Home/ReaderReport/index.vue";
import UserInfor from "../pages/Home/UserInfor/index.vue";
import Person from "../pages/Home/Person/index.vue";
import AdminReport from "../pages/Home/AdminReport/index.vue";
import AdminReserve from "../pages/Home/AdminReserve/index.vue";
import AdminBorrows from "../pages/Home/AdminBorrows/index.vue";
import BuyBook from "../pages/Home/BuyBook/index.vue";
import SupplierInfor from "../pages/Home/SupplierInfor/index.vue";


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
          //     个人信息
            path: "userinfor",
            component: UserInfor,
          },
          {
              //     人员管理
              path: "person",
              component: Person,
          },
          {
              //     管理员反馈
              path: "adminreport",
              component: AdminReport,
          },
          {
              //     管理员预约记录
              path: "adminreserve",
              component: AdminReserve,
          },
          {
              //     管理员借书记录
              path: "adminborrows",
              component: AdminBorrows,
          },
          {
              //     管理员购买记录
              path: "buybook",
              component: BuyBook,
          },
          {
              //     供应商记录
              path: "supplierinfor",
              component: SupplierInfor,
          }
      ],
    },
  ],
});
