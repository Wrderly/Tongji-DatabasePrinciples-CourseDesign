import Vue from "vue";
import App from "./App.vue";
// 引入路由
import router from "@/router";

// 引入vuex
import store from "@/store";

// 引入element-ui
import ElementUI from "element-ui";
import "element-ui/lib/theme-chalk/index.css";
Vue.use(ElementUI);

// 引入时间格式化库
// import moment from 'moment'
// Vue.prototype.$moment = moment;
import timeFormater from "time-formater";
Vue.prototype.$moment = timeFormater;

// 引入全局组件
// 读者侧边栏
import ReaderBanner from "@/components/ReaderBanner";
Vue.component(ReaderBanner.name, ReaderBanner);
// 管理员侧边栏
import AdminBanner from '@/components/AdminBanner'
Vue.component(AdminBanner.name, AdminBanner)

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount("#app");
