<template>
    <div class="clearfix">
        <header>
            <div>
                <span>图书管理系统</span>
            </div>
            <el-dropdown @command="handleCommand">
                <span class="el-dropdown-link">
                    欢迎&nbsp;{{ user_name }}&nbsp;登录图书管理中心<i class="el-icon-arrow-down el-icon--right"></i>
                </span>
                <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item command="profile">个人信息</el-dropdown-item>
                </el-dropdown-menu>
            </el-dropdown>
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
    import { mapState } from "vuex";
    export default {
        name: "HoMe",
        data() {
            return {};
        },
        computed: {
            ...mapState({
                isAdmin(state) {
                    return state.User.isAdmin;
                },
                user_name(state) {
                    if (this.isAdmin) {
                        return state.User.adminInfo.admin_name;
                    } else {
                        return state.User.readerInfo.reader_name;
                    }
                },
            }),
        },
        methods: {
            toggleUser() {
                this.$router.push("/LoginRegister");
            },
            handleCommand(command) {
                if (command === 'profile') {
                    this.$router.push("/home/userinfor");
                }
            },
        },
    };
</script>

<style lang="less" scoped>
    header {
        background-color: #373d41;
        display: flex;
        //设置显示为flex布局 
        justify-content: space-between;
        //设置为flex左右布局 
        padding-left: 0;
        //左内边距为0 
        align-items: center;
        //元素上下居中（防止右边按钮贴上下边） 
        color: #fff;
        font-size: 20px;
        margin-bottom: 60px;
        > div{
        display: flex;
        align-items: center;
        //文字上下居中 
        span{
        margin-left: 15px;  //文字左侧设置间距
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

    .el-dropdown-link {
        color: #fff;
        margin-right: 0;
    }
</style>
