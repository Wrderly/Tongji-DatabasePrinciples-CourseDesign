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
            <template slot-scope="scope">
                <el-button type="danger" @click="showDialog(scope.$index)">修改</el-button>
                <el-dialog title="修改违约次数" :visible.sync="dialogVisible">
                    <span>请输入新的违约次数：</span>
                    <el-input-number v-model="newCount" :min="0" :max="5"></el-input-number>
                    <span slot="footer" class="dialog-footer">
                        <el-button @click="dialogVisible = false">取消</el-button>
                        <el-button type="primary" @click="UpdateCount">确定</el-button>
                    </span>
                </el-dialog>
            </template>
        </el-table-column>
    </el-table>
</template>

<script>
import { mapState } from "vuex";
import { updateCount } from "@/api";
export default {
  data() {
        return {
            readerId:0,
            dialogVisible: false, // 对话框显示状态
      newCount:0,
      loading: false,
    };
  },
  name: "Person",
  mounted() {
    this.$store.dispatch("initReaderList");
  },
        methods: {
            showDialog(index) {
                this.readerId = this.readerList[index].READER_ID;
                this.newCount = this.readerList[index].OVERDUE_TIMES; // 设置初始值为当前行的违约次数
                this.dialogVisible = true;
            },
    // 修改违约次数
    UpdateCount() {
        let reader_id = this.readerId;
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
              this.dialogVisible = false;
              this.newCount = 0;
              this.readerId = 0;
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
      readerList(state) {
          return state.User.readerList;
      },
    }),
  },
};
</script>

<style lang="less" scoped></style>