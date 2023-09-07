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
        <el-table-column prop="OVERDUE_TIMES" label="违约次数">
            <template slot-scope="props">
                <span>{{ props.row.OVERDUE_TIMES }}</span>
            </template>
        </el-table-column>
        <el-table-column label="修改违约次数">
            <el-button type="primary" @click="showDialog = true">修改</el-button>
            <el-dialog title="修改违约次数" :visible.sync="showDialog">
                <span>您确定要修改此用户的违约次数吗？</span>
                <el-input type="text" v-model="newCount" placeholder="请输入新的违约次数"></el-input>
                <span slot="footer" class="dialog-footer">
                    <el-button @click="showDialog = false">取消</el-button>
                    <el-button type="primary" @click="updateCount(scope.$index, scope.row)">确定</el-button>
                </span>
            </el-dialog>
        </el-table-column>
    </el-table>
</template>

<script>
import { mapState } from "vuex";
import { updateCount } from "@/api";
export default {
  data() {
    return {

      newCount:'',
      showDialog:false,
      loading: false,
    };
  },
  name: "Person",
  mounted() {
    this.$store.dispatch("initReaderList");
  },
  methods: {
    // 修改违约次数
    UpdateCount(index, row) {
      console.log(index, row);
      let reader_id = row.READER_ID;
      let overdue_times = this.newCount;
      let Obj = { reader_id, overdue_times };
      console.log(Obj);
      this.loading = true;
          updateCount(Obj).then(
        (res) => {
          this.loading = false;
          console.log(res);
          if (res.status == 200) {
            this.$message({
              showClose: true,
              message: "修改成功！",
              type: "success",
            });
            this.showDialog = false;
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