<template>
    <el-table :data="borrows"
              style="width: 100%"
              height="450"
              v-loading.fullscreen.lock="loading"
              element-loading-text="正在处理..."
              element-loading-spinner="el-icon-loading"
              element-loading-background="rgba(0, 0, 0, 0.8)">
        <el-table-column type="expand">
            <template slot-scope="props">
                <el-form label-position="left" class="demo-table-expand">
                    <el-form-item label="借书日期：">
                        <span>{{ props.row.BORROW_DATE }}</span>
                    </el-form-item>
                    <el-form-item label="还书日期："
                                  v-if="props.row.RETURN_DATE != 'NULL'">
                        <span>{{ props.row.RETURN_DATE }}</span>
                    </el-form-item>
                    <el-form-item label="图书状态：">
                        <span>{{ props.row.MESSAGE }}</span>
                    </el-form-item>
                    <el-form-item label="续借次数："
                                  v-if="props.row.RENEW_TIME != -1">
                        <span>{{ props.row.RENEW_TIME }}</span>
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
                <span>《{{ props.row.BOOK_NAME }}》</span>
            </template>
        </el-table-column>
        <el-table-column prop="author" label="图书作者">
            <template slot-scope="props">
                <span>{{ props.row.AUTHOR }}</span>
            </template>
        </el-table-column>

        <el-table-column label="操作" width="200">
            <template slot-scope="scope">
                <el-popconfirm title="确认归还该书籍吗？"
                               @confirm="returnBook(scope.$index, scope.row)"
                               v-if="scope.row.MESSAGE != '已还书'">
                    <el-button size="mini"
                               type="primary"
                               plain
                               style="margin-right: 10px"
                               slot="reference">
                        还书
                    </el-button>
                </el-popconfirm>

                <el-popconfirm title="确认续借该书籍吗？"
                               @confirm="continueBorrowBook(scope.$index, scope.row)"
                               v-if="scope.row.MESSAGE != '已还书'">
                    <el-button size="mini"
                               type="success"
                               :plain="scope.row.MESSAGE == '借阅中'"
                               slot="reference">
                        续借
                    </el-button>
                </el-popconfirm>

                <el-button size="mini" disabled v-if="scope.row.MESSAGE == '已还书'">
                    已还
                </el-button>
            </template>
        </el-table-column>
    </el-table>
</template>

<script>
    import { mapState } from "vuex";
    import { continueBorrow, returnBook, BorrowOvertime } from "@/api";
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
                console.log(index, row);
                let infoObj = {
                    book_id: row.BOOK_ID,
                    reader_id: this.reader_id,
                    borrow_date: row.BORROW_DATE,
                    return_date: this.$moment().format("YYYY-MM-DD HH:mm:ss"),
                    message: "已还书"
                };
                returnBook(infoObj).then(
                    (res) => {
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
                                message: "还书成功",
                                type: "success",
                            });
                        }
                        this.$store.dispatch("initBorrows", { reader_id: this.reader_id });
                        this.$store.dispatch("initReserve", { reader_id: this.reader_id });
                        this.$store.dispatch("initBooksList");
                    },
                    (err) => {
                        console.log(err.message);
                    }
                );
            },
            // 续借
            continueBorrowBook(index, row) {
                console.log(index, row);
                this.loading = true;
                let infoObj = {
                    reader_id: this.reader_id,
                    book_id: row.BOOK_ID,
                    borrow_date: this.$moment().format("YYYY-MM-DD HH:mm:ss"),
                    message: row.MESSAGE,
                    old_borrow_date: row.BORROW_DATE,
                };
                continueBorrow(infoObj).then((res) => {
                    this.loading = false;
                    console.log(res);
                    if (res.status == 200) {
                        this.$message({
                            showClose: true,
                            message: "续借成功！",
                            type: "success",
                        });
                    }
                    else {
                        this.$message({
                            showClose: true,
                            message: res.msg,
                            type: "error",
                        });
                    }
                    this.$store.dispatch("initBorrows", { reader_id: this.reader_id });
                });
            },
        },
        computed: {
            ...mapState({
                borrows(state) {
                    return state.Borrows.borrows;
                },
                reader_id(state) {
                    return state.User.readerInfo.reader_id;
                },
            }),
        },
        mounted() {
            let data = {
                reader_id: this.reader_id,
                now_time: this.$moment().format("YYYY-MM-DD HH:mm:ss"),
            }
            BorrowOvertime(data);
            this.$store.dispatch(
                "initBorrows",
                { reader_id: this.reader_id }
            );
        },
    };
</script>

<style lang="less" scoped></style>
