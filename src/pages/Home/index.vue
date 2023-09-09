<template>
  <div class="clearfix">
    <header>
      <div>
        <span>图书管理系统</span>
      </div>
      <el-button type="info" @click="toggleUser">退出登录</el-button>
    </header>

    <el-row :gutter="10">
      <el-col :span="6">
        <AdminBanner v-if="isAdmin" />
        <ReaderBanner v-else />
      </el-col>
      <el-col :span="17">
        <div class="tablemain">
          <router-view />
        </div>
      </el-col>
    </el-row>
  </div>
</template>
<script>
import axios from "axios";
import { mapState } from "vuex";
export default {
  name: "Home",
  data() {
    return {};
  },
  computed: {
    ...mapState({
      isAdmin(state) {
        return state.User.isAdmin;
      },
      userName(state) {
        if (this.isAdmin) {
          return state.User.adminName;
        } else {
          return state.User.readerName;
        }
      },
      readerId(state) {
        return state.User.readerInfo.readerId;
      },
    }),
  },
  methods: {
    toggleUser() {
      this.$router.push("/LoginRegister");
    },
  },
};
</script>

<style lang="less" scoped>
header {
  background-color: #373d41;
  display: flex; //设置显示为flex布局
  justify-content: space-between; //设置为flex左右布局
  padding-left: 0; //左内边距为0
  align-items: center; //元素上下居中（防止右边按钮贴上下边）
  color: #fff;
  font-size: 20px;
  margin-bottom: 60px;
  > div {
    //内嵌的div样式
    display: flex;
    align-items: center; //文字上下居中
    span {
      margin-left: 15px; //文字左侧设置间距
    }
  }
}
.tablemain {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  border: 1px solid #eee;
  border-radius: 1px 1px 1px;
  padding: 70px;
  min-height: 450px;
}
</style>
