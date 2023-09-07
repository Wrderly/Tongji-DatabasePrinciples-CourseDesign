<template>
  <div>
    <el-input
      placeholder="请输入您要搜索的书名/作者/ISBN"
      prefix-icon="el-icon-search"
      @keyup.enter.native="searchBook"
      @blur="clear"
      v-model="name"
    >
    </el-input>
    <el-table :data="flag == 0 ? booksList : searchBooks"
              height="450"
              style="width: 100%"
              v-loading.fullscreen.lock="loading"
              element-loading-text="正在处理..."
              element-loading-spinner="el-icon-loading"
              element-loading-background="rgba(0, 0, 0, 0.8)">
        <el-table-column type="expand">
            <template slot-scope="props">
                <el-form label-position="left" class="demo-table-expand">
                    <el-form-item label="书籍介绍：">
                        <span>{{ props.row.INTRODUCTION }}</span>
                        <br>
                        <el-popconfirm title="确认修改该书籍吗？"
               v-if="isAdmin"
               style="float: left;"
               @confirm="ChangeBookInfo(props.row)">
    <el-button size="mini" type="primary" slot="reference">修改</el-button>
</el-popconfirm>
                        <el-popconfirm title="确认删除该书籍吗？"
                                       v-if="isAdmin"
                                       style="float: right;"
                                       @confirm="delBook(props.row)">
                            &nbsp;<el-button size="mini" type="danger" slot="reference">删除书籍</el-button>
                        </el-popconfirm>
                    </el-form-item>
                </el-form>
            </template>
        </el-table-column>
        <el-table-column sortable label="图书名称" prop="book_name">
            <template slot-scope="props">
                <span>{{ props.row.BOOK_NAME }}</span>
            </template>
        </el-table-column>
        <el-table-column sortable label="图书作者" prop="author">
            <template slot-scope="props">
                <span>{{ props.row.AUTHOR }}</span>
            </template>
        </el-table-column>
        <el-table-column label="书籍ISBN" prop="ISBN">
            <template slot-scope="props">
                <span>{{ props.row.ISBN }}</span>
            </template>
        </el-table-column>
        <el-table-column label="图书类别" prop="collection_type">
            <template slot-scope="props">
                <span>{{ props.row.COLLECTION_TYPE }}</span>
            </template>
        </el-table-column>
        <el-table-column sortable label="当前库存" prop="num">
            <template slot-scope="props">
                <span>{{ props.row.NUM }}</span>
            </template>
        </el-table-column>
        <el-table-column label="操作" v-if="!isAdmin">
            <template slot-scope="scope">
                <el-button size="mini"
                           type="primary"
                           plain
                           @click="bookReserve(scope.$index, scope.row)">预约</el-button>
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
      let reader_id = this.reader_id;
      let book_id = row.BOOK_ID;
      let reserve_date = this.$moment().format("YYYY-MM-DD HH:mm:ss");
      let book_name = row.BOOK_NAME;
      let author = row.AUTHOR;
      let isbn = row.ISBN;
      let reserveObj = { reader_id, book_id, reserve_date, message: "已预约", book_name, author, isbn };
      console.log(reserveObj);
      //  添加预约记录
      addReserve(reserveObj).then(
        (res) => {
          this.loading = false;
              console.log(res);
          if(res.status == 200) {
        this.$message({
            showClose: true,
            message: "预约成功",
            type: "success",
        });
    }
          else if (res.status == 0) {
            this.$message({
              showClose: true,
              message: res.msg,
              type: "error",
            });
              } 
              let data = {
                  reader_id: this.reader_id
              };
              this.$store.dispatch("initReserve", data);
              this.$store.dispatch("initBooksList");
        },
        (err) => {
          this.loading = false;
          console.log(err.message);
        }
      );
    },
    searchBook(e) {
        this.loading = true;
        let data = {
            searchStr:this.name,
        };
      searchBook(data).then(
        (res) => {
          this.loading = false;
          e.target.blur();
          this.flag = 1;
          this.searchBooks = res.books;
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
     ChangeBookInfo(row) {
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
      reader_id(state) {
        return state.User.readerInfo.reader_id;
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
