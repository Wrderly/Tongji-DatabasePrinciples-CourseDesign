<template>
  <el-table
    :data="reserve"
    style="width: 100%"
    height="450"
    v-loading.fullscreen.lock="loading"
    element-loading-text="正在处理..."
    element-loading-spinner="el-icon-loading"
    element-loading-background="rgba(0, 0, 0, 0.8)"
  >
    <el-table-column type="expand">
      <template slot-scope="props">
        <el-form label-position="left" class="demo-table-expand">
          <el-form-item label="预约日期：">
            <span>{{ props.row.date }}</span>
          </el-form-item>
          <el-form-item label="书籍名称：">
            <span>{{ props.row.bookName }}</span>
          </el-form-item>
          <el-form-item label="图书作者：">
            <span>{{ props.row.author }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column prop="date" label="预约日期"> </el-table-column>
    <el-table-column prop="bookName" label="书籍名称"> </el-table-column>
    <el-table-column prop="author" label="图书作者"> </el-table-column>

    <el-table-column label="操作" width="200">
      <template slot-scope="scope">
        <el-popconfirm
          title="确认取消该预约吗？"
          style="margin-right: 10px"
          @confirm="cancelReserve(scope.$index, scope.row)"
          v-if="scope.row.status == '已预约'"
        >
          <el-button size="mini" type="warning" plain slot="reference"
            >取消预约
          </el-button>
        </el-popconfirm>
        <el-popconfirm
          title="确认借阅该书籍吗？"
          @confirm="confirmBorrow(scope.$index, scope.row)"
          v-if="scope.row.status == '已预约'"
        >
          <el-button size="mini" type="primary" plain slot="reference"
            >确认借书
          </el-button>
        </el-popconfirm>

        <el-button size="mini" disabled v-if="scope.row.status == '已借阅'"
          >已借阅
        </el-button>
      </template>
    </el-table-column>
  </el-table>
</template>

<script>
import { mapState } from "vuex";
import { deleteReserve, addBorrow } from "@/api";
export default {
  data() {
    return {
      loading: false,
    };
  },
  name: "ReaderSubscribe",
  methods: {
    // 取消预约
    cancelReserve(index, row) {
      console.log(index, row);
      let bookId = row.bookId;
      let readerId = this.$store.state.User.readerInfo.readerId;
      let date = row.date;
      let obj = { bookId, readerId, date };
      console.log(obj);
      this.loading = true;
      deleteReserve(JSON.stringify(obj)).then(
        (res) => {
          this.loading = false;
          console.log(res);
          if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "取订成功！",
              type: "success",
            });
          }
          this.$store.dispatch("initReserve", { readerId: this.readerId });
        },
        (err) => {
          this.loading = false;
          console.log(err.message);
        }
      );
    },
    // 确认借书
    confirmBorrow(index, row) {
      console.log(index, row);
      let readerId = this.readerId;
      let bookId = row.bookId;
      let date = row.date;

      this.loading = true;

      let borrowObj = { readerId, bookId, date };
      //  添加借书记录
      addBorrow(JSON.stringify(borrowObj)).then(
        (res) => {
          this.loading = false;
          console.log(res);
          if (res.status == 0) {
            this.$message({
              showClose: true,
              message: res.msg,
              type: "error",
            });
          } else if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "借书成功！",
              type: "success",
            });
          }
          this.$store.dispatch("initBorrows", { readerId: this.readerId });
          this.$store.dispatch("initReserve", { readerId: this.readerId });
        },
        (err) => {
          this.loading = false;
          console.log(err.message);
        }
      );
    },
  },
  computed: {
    ...mapState({
      reserve(state) {
        return state.Reserve.reserve;
      },
      readerId(state) {
        return state.User.readerInfo.readerId;
      },
    }),
  },
  mounted() {
    this.$store.dispatch("initReserve", { readerId: this.readerId });
  },
};
</script>

<style lang="less" scoped></style>
