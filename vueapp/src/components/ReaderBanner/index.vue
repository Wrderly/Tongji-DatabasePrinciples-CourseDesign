<template>
  <div class="main">
    <el-radio-group v-model="isCollapse" style="margin-bottom: 20px">
      <el-radio-button :label="false">展开</el-radio-button>
      <el-radio-button :label="true">收起</el-radio-button>
    </el-radio-group>
    <el-menu
      default-active="1"
      class="el-menu-vertical-demo"
      :collapse="isCollapse"
      @open="handleOpen"
      @close="handleClose"
      active-text-color="#e60000"
    >
      <el-menu-item index="1" @click="$router.push('/home/userhome')">
        <i class="el-icon-s-home"></i>
        <span>首页</span>
      </el-menu-item>
      <el-submenu index="2">
        <template slot="title">
          <i class="el-icon-search"></i>
          <span>预约管理</span>
        </template>
        <el-menu-item index="1-1" @click="$router.push('/home/readerreserve')">
          <span>预约记录</span>
        </el-menu-item>
        <el-menu-item index="1-2" @click="$router.push('/home/search')">
          <span>书籍查找及预约</span>
        </el-menu-item>
      </el-submenu>
      <el-submenu index="3">
        <template slot="title">
          <i class="el-icon-reading"></i>
          <span>借阅管理</span>
        </template>
        <el-menu-item index="1-1" @click="$router.push('/home/readerborrows')">
          <span>借阅记录及续借</span>
        </el-menu-item>
      </el-submenu>
      <el-menu-item index="4" @click="$router.push('/home/readerreport')">
        <i class="el-icon-chat-line-round"></i>
        <span>反馈</span>
      </el-menu-item>
      <el-submenu index="5">
        <template slot="title">
          <i class="el-icon-user-solid"></i>
          <span>用户</span>
        </template>
        <el-menu-item index="1-1" @click="$router.push('/home/userinfor')">
          <span>个人信息</span>
        </el-menu-item>
        <el-menu-item index="1-2" @click="toggleUser">
          <span slot="title">退出账号</span>
        </el-menu-item>
      </el-submenu>
    </el-menu>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  name: "ReaderBanner",
  data() {
    return {
      isCollapse: true,
    };
  },
  mounted() {
    // this.$store.dispatch('initCommentsList')
    // this.$store.dispatch('initBorrows',{readerId:this.readerId})
    // this.$store.dispatch('initReserve',{readerId:this.readerId})
    // this.$store.dispatch('initStuReport',{readerId:this.readerId})
  },
  computed: {
    ...mapState({
      readerId(state) {
        return state.User.readerInfo.readerId;
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
      },
  },
};
</script>

<style lang="less" scoped>
.main {
  float: left;
  margin-right: 200px;
}
</style>
