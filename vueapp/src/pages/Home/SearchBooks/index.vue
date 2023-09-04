<template>
  <div>
    <el-input
      placeholder="请输入您要搜索的书名/作者"
      prefix-icon="el-icon-search"
      @keyup.enter.native="searchBook"
      @blur="clear"
      v-model="name"
    >
    </el-input>
    <el-table
      :data="flag == 0 ? booksList : searchBooks"
      height="450"
      style="width: 100%"
      v-loading.fullscreen.lock="loading"
      element-loading-text="正在处理..."
      element-loading-spinner="el-icon-loading"
      element-loading-background="rgba(0, 0, 0, 0.8)"
    >
      <el-table-column type="expand">
        <template slot-scope="props">
          <el-form label-position="left" class="demo-table-expand">
            <el-form-item label="图书名称：">
              <span>{{ props.row.bookName }}</span
              >&nbsp;<el-button
                v-show="isAdmin"
                @click="changeBookName(props.row)"
                type="text"
                style="float: right"
                size="mini"
                icon="el-icon-edit"
                >修改</el-button
              >
            </el-form-item>
            <el-form-item label="图书作者：">
              <span>{{ props.row.author }}</span
              >&nbsp;<el-button
                v-show="isAdmin"
                @click="changeBookAuthor(props.row)"
                type="text"
                style="float: right"
                size="mini"
                icon="el-icon-edit"
                >修改</el-button
              >
            </el-form-item>
            <el-form-item label="书籍ISBN：">
              <span>{{ props.row.isbn }}</span
              >&nbsp;<el-button
                v-show="isAdmin"
                @click="changeBookPosition(props.row)"
                type="text"
                style="float: right"
                size="mini"
                icon="el-icon-edit"
                >修改</el-button
              >
            </el-form-item>
            <el-form-item label="图书类别：">
              <span>{{ props.row.category }}</span>
              &nbsp;<el-button
                v-show="isAdmin"
                @click="changeCurrentCategory(props.row)"
                type="text"
                style="float: right"
                size="mini"
                icon="el-icon-edit"
                >修改</el-button
              >
            </el-form-item>
            <el-form-item label="借阅次数：">
              <span>{{ props.row.borrowedTimes }}</span>
              <el-popconfirm
                title="确认删除该书籍吗？"
                v-if="isAdmin"
                style="float: right"
                @confirm="delBook(props.row)"
              >
                <el-button size="mini" type="danger" slot="reference"
                  >删除书籍</el-button
                >
              </el-popconfirm>
            </el-form-item>
          </el-form>
        </template>
      </el-table-column>
      <el-table-column label="图书名称" sortable prop="bookName">
      </el-table-column>
      <el-table-column sortable label="图书作者" prop="author">
      </el-table-column>
      <el-table-column sortable label="书籍ISBN" prop="position">
      </el-table-column>
      <el-table-column sortable label="图书类别" prop="category">
      </el-table-column>
      <el-table-column label="操作" v-if="!isAdmin">
        <template slot-scope="scope">
          <el-button
            size="mini"
            type="primary"
            plain
            @click="bookReserve(scope.$index, scope.row)"
            >预约</el-button
          >
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { mapState } from "vuex";
import {
  addReserve,
  initReserve,
  searchBook,
  changeBookInfo,
  delBook,
} from "@/api";
export default {
  name: "SearchBooks",
  data() {
    return {
      loading: false,
      name: "",
      flag: 0,
      borrowInfo: {
        borrowDate: "",
        realDate: "",
      },
      searchBooks: [],
    };
  },
  methods: {
    bookReserve(index, row) {
      this.loading = true;
      console.log(index, row);
      let readerId = this.readerId;
      let bookId = row.bookId;
      let date = this.$moment().format("YYYY-MM-DD HH:mm:ss");
      let reserveObj = { readerId, bookId, date, status: "已预约" };
      console.log(reserveObj);
      //  添加预约记录
      addReserve(JSON.stringify(reserveObj)).then(
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
              message: res.msg,
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
    searchBook(e) {
      this.loading = true;
      searchBook(JSON.stringify({ name: this.name })).then(
        (res) => {
          this.loading = false;
          e.target.blur();
          this.flag = 1;
          this.searchBooks = res.data;
          console.log(res);
          if (res.status == 0) {
            this.$message({
              showClose: true,
              message: "未找到相关书籍！",
              type: "error",
            });
          }
        },
        (err) => {
          this.loading = false;
          console.log(err.message);
        }
      );
    },
    clear() {
      this.flag = 0;
      this.searchBooks = [];
    },
    changeBookName(row) {
      console.log(row);
      var bookId = row.bookId;
      var status = 1;
      this.$prompt("请输入书名", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputValue: row.bookName,
      })
        .then(({ value }) => {
          this.$message({
            type: "success",
            message: "你修改的书名是: " + value,
          });
          // 修改的信息
          var infoObj = { bookId, value, status };
          changeBookInfo(JSON.stringify(infoObj)).then(
            (res) => {
              console.log(res);
              this.$store.dispatch("initBooksList");
              this.$store.dispatch("initReserveList");
            },
            (err) => {
              console.log(err.message);
            }
          );
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "取消输入",
          });
        });
    },
    changeBookAuthor(row) {
      console.log(row);
      var bookId = row.bookId;
      var status = 2;
      this.$prompt("请输入作者名", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputValue: row.author,
      })
        .then(({ value }) => {
          this.$message({
            type: "success",
            message: "你修改的作者名是: " + value,
          });
          // 修改的信息
          var infoObj = { bookId, value, status };
          changeBookInfo(JSON.stringify(infoObj)).then(
            (res) => {
              console.log(res);
              this.$store.dispatch("initBooksList");
              this.$store.dispatch("initReserveList");
            },
            (err) => {
              console.log(err.message);
            }
          );
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "取消输入",
          });
        });
    },
    changeBookPosition(row) {
      console.log(row);
      var bookId = row.bookId;
      var status = 3;
      this.$prompt("请输入ISBN", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputValue: row.isbn,
      })
        .then(({ value }) => {
          // 修改的信息
          var infoObj = { bookId, value, status };
          changeBookInfo(JSON.stringify(infoObj)).then(
            (res) => {
              console.log(res);
              if (res.status == 0) {
                this.$message({
                  type: "error",
                  message: res.msg,
                });
              } else {
                this.$message({
                  type: "success",
                  message: "你修改的ISBN是: " + value,
                });
              }
              this.$store.dispatch("initBooksList");
              this.$store.dispatch("initReserveList");
            },
            (err) => {
              console.log(err.message);
            }
          );
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "取消输入",
          });
        });
    },
    changeCurrentCategory(row) {
      console.log(row);
      var bookId = row.bookId;
      var status = 4;
      this.$prompt("请输入库类别", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        inputValue: row.category,
      })
        .then(({ value }) => {
          this.$message({
            type: "success",
            message: "你修改当前类别是: " + value,
          });
          // 修改的信息
          var infoObj = { bookId, value, status };
          changeBookInfo(JSON.stringify(infoObj)).then(
            (res) => {
              console.log(res);
              this.$store.dispatch("initBooksList");
              this.$store.dispatch("initReserveList");
            },
            (err) => {
              console.log(err.message);
            }
          );
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "取消输入",
          });
        });
    },
    delBook(row) {
      console.log(row);
      let bookId = row.bookId;
      delBook(JSON.stringify({ bookId })).then(
        (res) => {
          console.log(res);
          if (res.status == 200)
            this.$message({
              type: "success",
              message: res.msg,
            });
          this.$store.dispatch("initBooksList");
          this.$store.dispatch("initReserveList");
        },
        (err) => {
          console.log(err.message);
        }
      );
    },
  },
  computed: {
    ...mapState({
      booksList(state) {
        return state.Books.booksList;
      },
      readerId(state) {
        return state.User.readerInfo.readerId;
      },
      isAdmin(state) {
        return state.User.isAdmin;
      },
    }),
  },
  mounted() {
    this.$store.dispatch("initBooksList");
    console.log(this.searchBooks);
  },
};
</script>

<style lang="less" scoped>
.demo-table-expand {
  font-size: 0;
}
.demo-table-expand label {
  width: 90px;
  color: #99a9bf;
}
.demo-table-expand .el-form-item {
  margin-right: 0;
  margin-bottom: 0;
  width: 50%;
}
</style>
