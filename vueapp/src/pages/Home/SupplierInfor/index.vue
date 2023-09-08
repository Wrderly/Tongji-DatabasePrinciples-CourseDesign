<template>
    <el-button type="primary" @click="showDialog = true">添加供应商</el-button>
    <el-dialog title="添加供应商" :visible.sync="showDialog">
        <el-form :model="form">
            <el-form-item label="供应商名">
                <el-input v-model="form.supplier_name" type="text" placeholder="Supplier"></el-input>
            </el-form-item>
            <el-form-item label="供应商手机号">
                <el-input v-model="form.phone_number" type="tel" placeholder="phoneNumber"></el-input>
            </el-form-item>
            <el-form-item label="供应商邮箱">
                <el-input v-model="form.email" type="email" placeholder="email"></el-input>
            </el-form-item>
            <el-form-item label="供应商地址">
                <el-input v-model="form.address" type="text" placeholder="address"></el-input>
            </el-form-item>
        </el-form>
        <span slot="footer" class="dialog-footer">
            <el-button @click="showDialog = false">取消</el-button>
            <el-button type="primary" @click="addSupplier">确定</el-button>
        </span>
    </el-dialog>
    <el-table :data="supplierList"
              style="width: 100%"
              height="450"
              v-loading.fullscreen.lock="loading"
              element-loading-text="正在处理..."
              element-loading-spinner="el-icon-loading"
              element-loading-background="rgba(0, 0, 0, 0.8)">
        <el-table-column prop="SUPPLIER_NAME" label="供应商名">
            <template slot-scope="props">
                <span>{{ props.row.SUPPLIER_NAME }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="PHONE_NUMBER" label="供应商手机号">
            <template slot-scope="props">
                <span>{{ props.row.PHONE_NUMBER }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="EMAIL" label="供应商邮箱">
            <template slot-scope="props">
                <span>{{ props.row.EMAIL }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="ADDRESS" label="供应商地址">
            <template slot-scope="props">
                <span>{{ props.row.ADDRESS }}</span>
            </template>
        </el-table-column>
        <el-table-column label="购买书籍">
            <template slot-scope="scope">
                <el-button size="mini"
                           type="success"
                           plain
                           @click="BuyBook(scope.$index, scope.row)">购买记录</el-button>
            </template>
        </el-table-column>
    </el-table>
</template>

<script>
import { mapState } from "vuex";
export default {
  data() {
        return {
      loading: false,
    };
  },
  name: "SupplierInfor",
  mounted() {
      this.$store.dispatch("initSupplierList");
  },
  methods: {
      addSupplier() {
            if (!this.form.supplier_name) {
        this.$message({
          showClose: true,
          message: "供应商名不能为空",
          type: "error",
        });
        return;
      } else if (!this.form.phone_number) {
        this.$message({
          showClose: true,
          message: "供应商手机号不能为空",
          type: "error",
        });
        return;
      } else if (!/^1[3456789]\d{9}$/.test(this.form.phone_number)) {
              this.$message({
                  showClose: true,
                  message: "手机号格式不正确！",
                  type: "error",
              });
              return;
      } else if (!this.form.email) {
        this.$message({
          showClose: true,
          message: "供应商邮箱不能为空",
          type: "error",
        });
        return;
      } else if (!/^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/.test(this.form.email)) {
              this.$message({
                  showClose: true,
                  message: "邮箱格式示例:'example@qq.com'",
                  type: "error",
              });
              return;
      } else if (!this.form.address) {
        this.$message({
          showClose: true,
          message: "供应商地址不能为空",
          type: "error",
        });
        return;
      }
        },
    bookComments(index, row){
        this.$store.dispatch("", row.SUPPLIER_ID);
        this.$router.push({ path: "/home/buybook", query: { SUPPLIER_ID: row.SUPPLIER_ID } });
    },
  },
  computed: {
    ...mapState({
      supplierList(state) {
            return state.Suppliers.supplierList;
      },
    }),
  },
};
</script>

<style lang="less" scoped></style>