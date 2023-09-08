<template>
    <div class="main">
        <el-radio-group v-model="isCollapse" style="margin-bottom: 20px;">
            <el-radio-button :label="false">展开</el-radio-button>
            <el-radio-button :label="true">收起</el-radio-button>
        </el-radio-group>
        <el-menu default-active="1" class="el-menu-vertical-demo" :collapse="isCollapse" @open="handleOpen"
                 @close="handleClose" active-text-color="#e60000">
            <el-menu-item index="1" @click="$router.push('/home/userhome')">
                <i class="el-icon-s-home"></i>
                <span>首页</span>
            </el-menu-item>
            <el-submenu index="2">
                <template slot="title">
                    <i class="el-icon-search"></i>
                    <span>用户预约管理</span>
                </template>
                <el-menu-item index="1-1" @click="$router.push('/home/adminreserve')">
                    <span>用户预约逾期</span>
                </el-menu-item>
                <el-menu-item index="1-2" @click="$router.push('/home/search')">
                    <span>书籍增加及修改</span>
                </el-menu-item>
            </el-submenu>
            <el-submenu index="3">
                <template slot="title">
                    <i class="el-icon-reading"></i>
                    <span>用户借阅管理</span>
                </template>
                <el-menu-item index="1-1" @click="$router.push('/home/adminborrows')">
                    <span>用户借阅逾期</span>
                </el-menu-item>
            </el-submenu>
            <el-menu-item index="4" @click="$router.push('/home/adminreport')">
                <i class="el-icon-chat-line-round"></i>
                <span slot="title">用户反馈</span>
            </el-menu-item>
            <el-menu-item index="5" @click="$router.push('/home/buybook')">
                <i class="el-icon-shopping-cart-full"></i>
                <span slot="title">书籍购买</span>
            </el-menu-item>
            <el-menu-item index="6" @click="$router.push('/home/supplierinfor')">
                <i class="el-icon-s-data"></i>
                <span slot="title">供应商信息</span>
            </el-menu-item>
            <el-submenu index="7">
                <template slot="title">
                    <i class="el-icon-user-solid"></i>
                    <span>用户</span>
                </template>
                <el-menu-item index="1-1" @click="$router.push('/home/person')">
                    <span>用户管理</span>
                </el-menu-item>
                <el-menu-item index="1-2" @click="$router.push('/home/userinfor')">
                    <span>个人信息</span>
                </el-menu-item>
                <el-menu-item index="1-3" @click="toggleUser">
                    <span slot="title">退出账号</span>
                </el-menu-item>
            </el-submenu>
        </el-menu>
    </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  name: "AdminBanner",
  data() {
    return {
      isCollapse: true
    };
  },

  mounted() {
    // this.$store.dispatch('initCommentsList')
    // this.$store.dispatch('initBorrowsList')
    // this.$store.dispatch('initReserveList')
    // this.$store.dispatch('initReaderList')
    // this.$store.dispatch('initReportList')
  },
    computed: {
    ...mapState({
      admin_id(state) {
        return state.User.adminInfo.admin_id;
      },
    }),
  },
  methods: {
    handleOpen() {
      console.log(1);
    },
    handleClose() {
      console.log(2);
    },
    toggleUser() {
      this.$router.push("/LoginRegister");
    }
  }
};
</script>

<style lang="less" scoped>
    .main {
        float: left;
        margin-right: 200px;
    }
</style>