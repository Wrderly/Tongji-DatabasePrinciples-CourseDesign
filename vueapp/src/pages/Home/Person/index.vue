<template>
    <el-table :data="readerList"
              style="width: 100%"
              height="450"
              v-loading.fullscreen.lock="loading"
              element-loading-text="正在处理..."
              element-loading-spinner="el-icon-loading"
              element-loading-background="rgba(0, 0, 0, 0.8)">
        <el-table-column prop="READER_NAME" label="用户名">
            <template slot-scope="props">
                <span>{{ props.row.READER_NAME }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="PHONE_NUMBER" label="用户手机号">
            <template slot-scope="props">
                <span>{{ props.row.PHONE_NUMBER }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="EMAIL" label="用户邮箱">
            <template slot-scope="props">
                <span>{{ props.row.EMAIL }}</span>
            </template>
        </el-table-column>
        <el-table-column label="操作">
            <template slot-scope="scope">
                <el-button size="mini"
                           type="primary"
                           plain
                           @click="deleteUser(scope.$index, scope.row)">删除</el-button>
            </template>
        </el-table-column>
    </el-table>
</template>

<script>
import { mapState } from "vuex";
import { deleteReserve } from "@/api";
export default {
  data() {
    return {
      loading: false,
    };
  },
  name: "ReaderReserve",
  mounted() {
    this.$store.dispatch("initReaderList");
  },
  methods: {
    // 删除用户
    deleteUser(index, row) {
      console.log(index, row);
      let reader_id = row.READER_ID;
      let reserveObj = { reader_id };
      console.log(reserveObj);
      this.loading = true;
          deleteReserve(reserveObj).then(
        (res) => {
          this.loading = false;
          console.log(res);
          if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "取消预约成功！",
              type: "success",
            });
          }
        },
        (err) => {
          this.loading = false;
          console.log(err.message);
        }
      );
      this.$store.dispatch("initReaderList");
    },
  },
  computed: {
    ...mapState({
      admin_id(state) {
          return state.User.adminInfo.admin_id;
      },
      readerList(state) {
          return state.User.readerList;
      },
    }),
  },
};
</script>

<style lang="less" scoped></style>