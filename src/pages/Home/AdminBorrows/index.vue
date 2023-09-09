<template>
  <div
    v-loading.fullscreen.lock="loading"
    :element-loading-text="loadingtext"
    element-loading-spinner="el-icon-loading"
    element-loading-background="rgba(0, 0, 0, 0.8)"
  >
    <el-input
      placeholder="请输入您要查找的用户姓名/图书名称"
      prefix-icon="el-icon-search"
      @keyup.enter.native="searchInfo"
      @blur="clear"
      v-model="info"
    >
    </el-input>
    <el-table
      :data="flag == 0 ? borrowsList : searchMessage"
      style="width: 100%"
      :default-sort="{ prop: 'date', order: 'descending' }"
    >
      <el-table-column type="expand">
        <template slot-scope="props">
          <el-form label-position="left" class="demo-table-expand">
            <el-form-item label="借阅日期：">
              <span>{{ props.row.borrowDate }}</span>
            </el-form-item>
            <el-form-item label="图书 ID："
              >&nbsp;
              <span>{{ props.row.bookId }}</span>
            </el-form-item>
            <el-form-item label="图书名称：">
              <span>{{ props.row.bookName }}</span>
            </el-form-item>
            <el-form-item label="读者编号：">
              <span>{{ props.row.readerId }}</span>
            </el-form-item>
            <el-form-item label="读者姓名：">
              <span>{{ props.row.readerName }}</span>
            </el-form-item>
            <el-form-item label="应还日期：">
              <span>{{ props.row.returnDate }}</span>
            </el-form-item>
            <el-form-item label="实际日期：">
              <span>{{ props.row.realDate }}</span>
              <el-popconfirm
                title="确认邮件提醒用户还书吗？"
                  @confirm="alertPerson(props.$index, props.row)"
              >
                <el-button
                  style="float: right; margin-right: 40px"
                  size="mini"
                  type="warning"
                  plain
                  slot="reference"
                  >提醒用户还书</el-button
                >
              </el-popconfirm>
            </el-form-item>
          </el-form>
        </template>
      </el-table-column>
      <el-table-column label="借阅日期" sortable prop="borrowDate">
        <template slot-scope="scope">
          <i class="el-icon-time"></i>
          <span style="margin-left: 10px">{{ scope.row.borrowDate }}</span>
        </template>
      </el-table-column>
      <el-table-column label="图书名称" sortable prop="bookName">
        <template slot-scope="scope">
          <el-popover trigger="hover" placement="top">
            <p>编号: {{ scope.row.bookId }}</p>
            <p>名称: {{ scope.row.bookName }}</p>
            <div slot="reference" class="name-wrapper">
              <el-tag size="medium">{{ scope.row.bookName }}</el-tag>
            </div>
          </el-popover>
        </template>
      </el-table-column>
      <el-table-column label="读者姓名" sortable prop="readerName">
        <template slot-scope="scope">
          <el-popover trigger="hover" placement="top">
            <p>编号: {{ scope.row.readerId }}</p>
            <p>姓名: {{ scope.row.readerName }}</p>
            <div slot="reference" class="name-wrapper">
              <el-tag size="medium">{{ scope.row.readerName }}</el-tag>
            </div>
          </el-popover>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template slot-scope="scope">
          <el-button
            size="mini"
            type="danger"
            @click="handleDelete(scope.$index, scope.row)"
            >删除记录</el-button
          >
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { mapState } from "vuex";
import { deleteBorrow, searchBorrow,alertPerson } from "@/api";
import qs from "qs";
export default {
  name: "AdminBorrows",
  data() {
    return {
      loading: false,
      loadingtext: "查询中...",
      info: "",
      searchMessage: [],
      flag: 0,
    };
  },
  computed: {
    ...mapState({
      borrowsList(state) {
        return state.Borrows.borrowsList;
      },
    }),
  },
  methods: {
    handleDelete(index, row) {
      console.log(row);
      let borrowObj = {
        readerId: row.readerId,
        bookId: row.bookId,
        borrowDate: row.borrowDate,
      };
      deleteBorrow(qs.stringify(borrowObj)).then(
        (res) => {
          console.log(res);
            this.$message({
              showClose: true,
              message: "删除借阅记录成功！",
              type: "success",
            });
          this.$store.dispatch("initBorrowsList");
        },
        (err) => {
          console.log(err.message);
        }
      );
    },
    searchInfo(e) {
      this.loading = true;
      searchBorrow(qs.stringify({ info: this.info })).then(
        (res) => {
          this.loading = false;
          e.target.blur();
          console.log(res);
          this.searchMessage = res.data;
          this.flag = 1;
          if (res.status == 0) {
            this.$message({
              showClose: true,
              message: "查询结果为空！",
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
      this.searchMessage = [];
    },
    alertPerson(index, row) {
      console.log(index, row);
      alertPerson(qs.stringify({readerId:row.readerId,bookName:row.bookName})).then(res=>{
        console.log(res);
      },err=>{
        console.log(err.message);
      })
    },
  },
  mounted(){
    this.$store.dispatch('initBorrowsList')
  }
};
</script>

<style lang="less" scoped>
</style>