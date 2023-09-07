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
            <span>{{ props.row.RESERVE_DATE }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column prop="ISBN" label="ISBN">
        <template slot-scope="props">
            <span>{{ props.row.ISBN }}</span>
        </template>
    </el-table-column>
    <el-table-column prop="bookName" label="书籍名称">
        <template slot-scope="props">
            <span>{{ props.row.BOOK_NAME }}</span>
        </template>
    </el-table-column>
    <el-table-column prop="author" label="图书作者">
        <template slot-scope="props">
            <span>{{ props.row.AUTHOR }}</span>
        </template>
    </el-table-column>

    <el-table-column label="操作" width="200">
        <template slot-scope="scope">
            <el-popconfirm title="确认取消该预约吗？"
                           style="margin-right: 10px"
                           @confirm="cancelReserve(scope.$index, scope.row)"
                           v-if="scope.row.MESSAGE == '已预约'">
                <el-button size="mini" type="warning" plain slot="reference">
                    取消预约
                </el-button>
            </el-popconfirm>
            <el-popconfirm title="确认借阅该书籍吗？"
                           @confirm="confirmBorrow(scope.$index, scope.row)"
                           v-if="scope.row.MESSAGE == '已预约'">
                <el-button size="mini" type="primary" plain slot="reference">
                    确认借书
                </el-button>
            </el-popconfirm>

            <el-button size="mini" disabled v-if="scope.row.MESSAGE == '借阅中'">
                已借阅
            </el-button>
            <el-button size="mini" disabled v-if="scope.row.MESSAGE == '逾期未取'">
                逾期未取
            </el-button>
        </template>
    </el-table-column>
  </el-table>
</template>

<script>
import { mapState } from "vuex";
import { deleteReserve, addBorrow, ReserveOvertime } from "@/api";
export default {
  data() {
    return {
      loading: false,
    };
  },
  name: "ReaderReserve",
  methods: {
    // 取消预约
    cancelReserve(index, row) {
      console.log(index, row);
      let book_id = row.BOOK_ID;
      let reader_id = this.reader_id;
      let reserveObj = { book_id, reader_id };
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
                  this.$store.dispatch("initReserve", { reader_id: this.reader_id });
                  this.$store.dispatch("initBooksList");
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
        let reader_id = this.reader_id;
        let book_id = row.BOOK_ID;
        let reserve_date = row.RESERVE_DATE;
        let borrow_date = this.$moment().format("YYYY-MM-DD HH:mm:ss");
        let book_name = row.BOOK_NAME;
        let author = row.AUTHOR;
        let isbn = row.ISBN;
        this.loading = true;
        let borrowObj = { reader_id, book_id, reserve_date, borrow_date, book_name, author, isbn, message: "已借阅" };
      //  添加借书记录
      addBorrow(borrowObj).then(
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
              this.$store.dispatch("initBorrows", { reader_id: this.reader_id });
              this.$store.dispatch("initReserve", { reader_id: this.reader_id });
              this.$store.dispatch("initBooksList");
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
      reader_id(state) {
          return state.User.readerInfo.reader_id;
      },
    }),
  },
   mounted() {
       let data = {
           reader_id: this.reader_id
       };
       //let data1 = {
       //    reader_id: this.reader_id,
       //    now_time = this.$moment().format("YYYY-MM-DD HH:mm:ss"),
       //}
       //ReserveOvertime(data1);
       this.$store.dispatch("initReserve", data);
  },
};
</script>

<style lang="less" scoped></style>
