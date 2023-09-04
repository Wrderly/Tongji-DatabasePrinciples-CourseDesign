<template>
    <div class="main">
        <el-radio-group v-model="isCollapse" style="margin-bottom: 20px;">
        <el-radio-button :label="false">展开</el-radio-button>
        <el-radio-button :label="true">收起</el-radio-button>
        </el-radio-group>
         <el-menu
      default-active="1"
      class="el-menu-vertical-demo"
      :collapse="isCollapse"
      @open="handleOpen"
      @close="handleClose"
      active-text-color="#ffd04b">
      <el-menu-item index="1" @click="$router.push('/home/introduce')"> 
        <i class="el-icon-s-home"></i>
        <span>首页</span>
      </el-menu-item>
       <el-menu-item index="2" @click="$router.push('/home/person')">
        <i class="el-icon-user-solid"></i>
        <span slot="title">人员管理</span>
      </el-menu-item>
       <el-menu-item index="3" @click="$router.push('/home/search')">
        <i class="el-icon-notebook-2"></i>
        <span slot="title">图书管理</span>
      </el-menu-item>
      <el-menu-item index="4" @click="$router.push('/home/adminborrows')">
        <i class="el-icon-search"></i>
        <span slot="title">借阅记录</span>
      </el-menu-item>
      <el-menu-item index="5" @click="$router.push('/home/adminsubcribe')">
        <i class="el-icon-s-promotion"></i>
        <span slot="title">预约记录</span>
      </el-menu-item>
      <el-menu-item index="6" @click="$router.push('/home/adminreport')">
        <i class="el-icon-message-solid"></i>
        <span slot="title">举报审核</span>
      </el-menu-item>
      <el-menu-item index="6" @click="$router.push('/home/comment')">
        <i class="el-icon-s-comment"></i>
        <span slot="title">交流社区</span>
      </el-menu-item>
      <el-menu-item index="7" @click="$router.push('/home/adminaddbooks')">
        <i class="el-icon-document-copy"></i>
        <span slot="title">添加图书</span>
      </el-menu-item>
     
      <el-submenu index="8">
        <template slot="title">
          <i class="el-icon-setting"></i>
          <span>设置</span>
        </template>
        <el-submenu index="1-1">
          <template slot="title">
          <i class="el-icon-user"></i>
                <span>我的</span>
          </template>
          <el-menu-item index="1-1-1" @click="toggleUser">
              <i class="el-icon-s-fold"></i>
              <span>切换账号</span>
        </el-menu-item>
        </el-submenu>
         <el-menu-item index="1-1-2" @click="toggleUser">
        <i class="el-icon-switch-button"></i>
        <span slot="title">退出账号</span>
      </el-menu-item>
      </el-submenu>
    </el-menu>
    </div>
</template>

<script>
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
.main{
    float: left;
    margin-right: 200px;
}
</style>