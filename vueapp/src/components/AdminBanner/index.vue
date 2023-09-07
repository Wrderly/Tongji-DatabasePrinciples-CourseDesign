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
                <el-menu-item index="1-1" @click="$router.push('/home/readerreserve')">
                    <span>用户预约记录</span>
                </el-menu-item>
                <el-menu-item index="1-2" @click="$router.push('/home/search')">
                    <span>书籍增加及修改</span>
                </el-menu-item>
            </el-submenu>
            <el-submenu index="3">
                <template slot="title">
                    <i class="el-icon-s-promotion"></i>
                    <span>用户借阅管理</span>
                </template>
                <el-menu-item index="1-1" @click="$router.push('/home/readerborrows')">
                    <span>用户借阅记录</span>
                </el-menu-item>
            </el-submenu>
            <el-menu-item index="4" @click="$router.push('/home/comment')">
                <i class="el-icon-s-comment"></i>
                <span slot="title">书籍评论</span>
            </el-menu-item>
            <el-menu-item index="5" @click="$router.push('/home/schedule')">
                <i class="el-icon-document-copy"></i>
                <span slot="title">排班表</span>
            </el-menu-item>
            <el-submenu index="6">
                <template slot="title">
                    <i class="el-icon-user"></i>
                    <span>用户</span>
                </template>
                <el-menu-item index="1-1" @click="$router.push('/home/adminreport')">
                    <span>用户反馈</span>
                </el-menu-item>
                <el-menu-item index="1-2" @click="$router.push('/home/person')">
                    <span>用户管理</span>
                </el-menu-item>
                <el-menu-item index="1-3" @click="$router.push('/home/userinfor')">
                    <span>个人信息</span>
                </el-menu-item>
                <el-menu-item index="1-4" @click="toggleUser">
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