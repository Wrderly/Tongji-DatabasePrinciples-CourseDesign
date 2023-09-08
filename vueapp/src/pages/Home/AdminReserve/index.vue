<template>
    <el-table :data="reserveList"
              style="width: 100%"
              height="450">
        <el-table-column type="expand">
            <template slot-scope="props">
                <el-form label-position="left" class="demo-table-expand">
                    <el-form-item label="预约日期：">
                        <span>{{ props.row.RESERVE_DATE }}</span>
                    </el-form-item>
                </el-form>
            </template>
        </el-table-column>
        <el-table-column prop="reader_id" label="读者ID">
            <template slot-scope="props">
                <span>{{ props.row.READER_ID }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="book_id" label="书籍ID">
            <template slot-scope="props">
                <span>{{ props.row.BOOK_ID }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="book_name" label="书籍名称">
            <template slot-scope="props">
                <span>{{ props.row.BOOK_NAME }}</span>
            </template>
        </el-table-column>
        <el-table-column prop="author" label="书籍作者">
            <template slot-scope="props">
                <span>{{ props.row.AUTHOR }}</span>
            </template>
        </el-table-column>
    </el-table>
</template>

<script>
import { mapState } from "vuex";
export default {
  name: "ReaderReserve",
  computed: {
    ...mapState({
      reserve(state) {
            return state.Reserve.reserveList;
      },
    }),
  },
   mounted() {
       this.$store.dispatch("initReserveList");
  },
};
</script>

<style lang="less" scoped></style>
