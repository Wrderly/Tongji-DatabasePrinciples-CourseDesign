<template>
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