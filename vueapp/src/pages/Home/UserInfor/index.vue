<template>
  <div>
      <el-descriptions title="用户信息" :column="1">
          <el-descriptions-item label="用户名" v-if="isAdmin">
              {{
        adminInfo.admin_name
              }}
          </el-descriptions-item>
          <el-descriptions-item label="用户名" v-else>
              {{
        readerInfo.reader_name
              }}
          </el-descriptions-item>
          <el-descriptions-item label="手机号" v-if="isAdmin">
              {{
        adminInfo.phone_number
              }}
          </el-descriptions-item>
          <el-descriptions-item label="手机号" v-else>
              {{
        readerInfo.phone_number
              }}
          </el-descriptions-item>
          <el-descriptions-item label="邮箱" v-if="isAdmin">
              {{
        adminInfo.email
              }}
          </el-descriptions-item>
          <el-descriptions-item label="邮箱" v-else>
              {{
        readerInfo.email
              }}
          </el-descriptions-item>
          <el-descriptions-item label="违规次数" v-if="!isAdmin">
              {{
        readerInfo.overdue_times
              }}
          </el-descriptions-item>
          <el-descriptions-item label="备注" v-if="isAdmin">
              <el-tag size="small">管理员</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="备注" v-else>
              <el-tag size="small">学生用户</el-tag>
          </el-descriptions-item>
      </el-descriptions>
    <el-button type="primary" @click="showDialog = true">重置密码</el-button>
    <el-dialog title="重置密码" :visible.sync="showDialog">
      <el-form :model="form">
        <el-form-item label="旧密码">
          <el-input v-model="form.oldPassword" type="password"></el-input>
        </el-form-item>
        <el-form-item label="新密码">
          <el-input v-model="form.newPassword" type="password"></el-input>
        </el-form-item>
        <el-form-item label="确认新密码">
          <el-input
            v-model="form.confirmNewPassword"
            type="password"
          ></el-input>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="showDialog = false">取消</el-button>
        <el-button type="primary" @click="resetPassword">确定</el-button>
      </span>
    </el-dialog>
    <el-button type="danger" @click="showDialog1 = true" v-show="!isAdmin">注销账户</el-button>
    <el-dialog title="注销账户" :visible.sync="showDialog1">
      <span>您确定要注销账户吗？</span>
      <span slot="footer" class="dialog-footer">
        <el-button @click="showDialog1 = false">取消</el-button>
        <el-button type="primary" @click="logout">确定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { mapState } from "vuex";
    import { initReader, change_pwd, logout_reader, initAdmin, change_pwd_admin } from "@/api";
    import sha256 from 'crypto-js/sha256';
export default {
        name: "UserInfor",
  data() {
    return {
      showDialog: false,
      showDialog1: false,
      form: {
        oldPassword: "",
        newPassword: "",
        confirmNewPassword: "",
      },
    };
  },
  computed: {
    ...mapState({
      isAdmin(state) {
        return state.User.isAdmin;
      },
        adminInfo(state) {
        if (this.isAdmin) 
            return state.User.adminInfo;
        else return {};
      },
      readerInfo(state) {
          if (!this.isAdmin)
              return state.User.readerInfo;
        else return {};
      },
    }),
  },
        mounted() {
            if (this.isAdmin)
                initAdmin({ admin_id: this.adminInfo.admin_id }).then(
                    (res) => {
                        console.log(res);
                        this.$store.dispatch("setAdminInfo", res);
                    },
                    (err) => {
                        console.log(err.message);
                    }
                );
      else
        initReader({ reader_id: this.readerInfo.reader_id }).then(
            (res) => {
                console.log(res);
                this.$store.dispatch("setReaderInfo", res);
            },
            (err) => {
                console.log(err.message);
            }
        );
  },
  methods: {
    resetPassword() {
      if (!this.form.oldPassword) {
        this.$message({
          showClose: true,
          message: "旧密码不能为空",
          type: "error",
        });
        return;
      } else if (!this.form.newPassword) {
        this.$message({
          showClose: true,
          message: "新密码不能为空",
          type: "error",
        });
        return;
      } else if (this.form.newPassword.length <= 6) {
              this.$message({
                  showClose: true,
                  message: "新密码长度至少为6位",
                  type: "error",
              });
              return;
          } else if (this.form.newPassword.length >= 18) {
              this.$message({
                  showClose: true,
                  message: "新密码长度最多为18位",
                  type: "error",
              });
              return;
          } else if (!/[a-z]/.test(this.form.newPassword) || !/[A-Z]/.test(this.form.newPassword)) {
              this.$message({
                  showClose: true,
                  message: "新密码必须包含大、小写字母",
                  type: "error",
              });
              return;
          } else if (!/\d/.test(this.form.newPassword)) {
              this.$message({
                  showClose: true,
                  message: "新密码必须包含数字",
                  type: "error",
              });
              return;
          } else if (this.form.newPassword === this.form.oldPassword) {
        this.$message({
          showClose: true,
          message: "新密码不能与旧密码一致",
          type: "error",
        });
        return;
      }     else if (this.form.newPassword !== this.form.confirmNewPassword) {
        this.$message({
          showClose: true,
          message: "新密码和确认新密码不一致",
          type: "error",
        });
        return;
      } 
      let encryptedPassword_oldPassword = sha256(this.form.oldPassword).toString();
      let encryptedPassword_newPassword = sha256(this.form.newPassword).toString();
      let encryptedPassword_confirmNewPassword = sha256(this.form.confirmNewPassword).toString();
      if(this.isAdmin)
      {
            let data = {
          admin_id: this.adminInfo.admin_id,
        oldPassword: encryptedPassword_oldPassword,
        newPassword: encryptedPassword_newPassword,
        confirmNewPassword: encryptedPassword_confirmNewPassword,
      };
      change_pwd_admin(data).then(
        (res) => {
          console.log(res);
          if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "密码修改成功！",
              type: "success",
            });
            this.form.oldPassword = "";
            this.form.newPassword = "";
            this.form.confirmNewPassword = "";
            this.showDialog = false;
          } else {
            this.form.oldPassword = "";
            this.form.newPassword = "";
            this.form.confirmNewPassword = "";
            this.$message({
              showClose: true,
              message: res.msg,
              type: "error",
            });
          }
        },
        (err) => {
          console.log(err.message);
          this.$message({
            showClose: true,
            message: "密码修改失败！",
            type: "error",
          });
          this.form.oldPassword = "";
          this.form.newPassword = "";
          this.form.confirmNewPassword = "";
        }
      );
      }
      else
      {
          let data = {
          reader_id: this.readerInfo.reader_id,
        oldPassword: encryptedPassword_oldPassword,
        newPassword: encryptedPassword_newPassword,
        confirmNewPassword: encryptedPassword_confirmNewPassword,
      };
      change_pwd(data).then(
        (res) => {
          console.log(res);
          if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "密码修改成功！",
              type: "success",
            });
            this.form.oldPassword = "";
            this.form.newPassword = "";
            this.form.confirmNewPassword = "";
            this.showDialog = false;
          } else {
            this.form.oldPassword = "";
            this.form.newPassword = "";
            this.form.confirmNewPassword = "";
            this.$message({
              showClose: true,
              message: res.msg,
              type: "error",
            });
          }
        },
        (err) => {
          console.log(err.message);
          this.$message({
            showClose: true,
            message: "密码修改失败！",
            type: "error",
          });
          this.form.oldPassword = "";
          this.form.newPassword = "";
          this.form.confirmNewPassword = "";
        }
      );
      }
    },
    logout() {
      let data1 = {
          reader_id: this.readerInfo.reader_id,
      };
      logout_reader(data1).then(
        (res) => {
          console.log(res);
          if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "用户注销成功！",
              type: "success",
            });
              this.showDialog1 = false;
              this.$router.push("/LoginRegister");
          } else {
            this.$message({
              showClose: true,
              message: res.msg,
              type: "error",
            });
          }
        },
        (err) => {
          console.log(err.message);
          this.$message({
            showClose: true,
            message: "用户注销失败！",
            type: "error",
          });
        }
      );
    },
  },
};
</script>

<style lang="less" scoped></style>
