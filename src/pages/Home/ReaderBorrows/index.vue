<template>
  <el-table
    :data="borrows"
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
          <el-form-item
            label="借书日期："
            v-if="props.row.borrowDate != '9999-12-31 00:00:00'"
          >
            <span>{{ props.row.borrowDate }}</span>
          </el-form-item>
          <el-form-item label="应还日期：">
            <span>{{ props.row.returnDate }}</span>
          </el-form-item>
          <el-form-item label="书籍名称：">
            <span>{{ props.row.bookName }}</span>
          </el-form-item>
          <el-form-item label="图书作者：">
            <span>{{ props.row.author }}</span>
          </el-form-item>
          <el-form-item label="图书状态：">
            <span>{{ props.row.status }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column prop="borrowDate" label="借阅日期"> </el-table-column>
    <el-table-column prop="bookName" label="书籍名称"> </el-table-column>
    <el-table-column prop="author" label="图书作者"> </el-table-column>

    <el-table-column label="操作" width="200">
      <template slot-scope="scope">
        <el-popconfirm
          title="确认归还该书籍吗？"
          @confirm="returnBook(scope.$index, scope.row)"
          v-if="scope.row.status != '已还'"
        >
          <el-button
            size="mini"
            type="primary"
            plain
            style="margin-right: 10px"
            slot="reference"
            >还书
          </el-button>
        </el-popconfirm>

        <el-popconfirm
          title="确认续借该书籍吗？"
          @confirm="continueBorrowBook(scope.$index, scope.row)"
          v-if="scope.row.status != '已还'"
        >
          <el-button
            size="mini"
            type="success"
            :plain="scope.row.status == '未还'"
            :disabled="scope.row.status == '续借'"
            slot="reference"
            >续借
          </el-button>
        </el-popconfirm>

        <el-button size="mini" disabled v-if="scope.row.status == '已还'"
          >已还
        </el-button>
      </template>
    </el-table-column>
  </el-table>
</template>

<script>
import { mapState } from "vuex";
import { continueBorrow, returnBook } from "@/api";
export default {
  name: "ReaderBorrow",
  data() {
    return {
      loading: false,
    };
  },
  methods: {
    // 还书
    returnBook(index, row) {
      console.log(row);
      let infoObj = {
        bookId: row.bookId,
        readerId: row.readerId,
        borrowDate: row.borrowDate,
      };
      returnBook(JSON.stringify(infoObj)).then(
        (res) => {
          console.log(res);
          if (res.status == 100) {
            this.$message({
              showClose: true,
              message: res.msg,
              type: "error",
            });
          } else if (res.status == 200) {
            this.$message({
              showClose: true,
              message: res.msg,
              type: "success",
            });
          }
          this.$store.dispatch(
            "initBorrows",
            JSON.stringify({ readerId: this.readerId })
          );
        },
        (err) => {
          console.log(err.message);
        }
      );
    },
    // 续借
    continueBorrowBook(index, row) {
      console.log(row);
      this.loading = true;
      let infoObj = {
        readerId: row.readerId,
        bookId: row.bookId,
        borrowDate: row.borrowDate,
        date: row.date,
      };
      continueBorrow(JSON.stringify(infoObj)).then((res) => {
        this.loading = false;
        console.log(res);
        if (res.status == 200) {
          this.$message({
            showClose: true,
            message: "续借成功！",
            type: "success",
          });
        }
        this.$store.dispatch(
          "initBorrows",
          JSON.stringify({ readerId: this.readerId })
        );
      });
    },
  },
  computed: {
    ...mapState({
      borrows(state) {
        return state.Borrows.borrows;
      },
      readerId(state) {
        return state.User.readerInfo.readerId;
      },
    }),
  },
  mounted() {
    this.$store.dispatch(
      "initBorrows",
      JSON.stringify({ readerId: this.readerId })
    );
  },
};
</script>

<style lang="less" scoped></style>
