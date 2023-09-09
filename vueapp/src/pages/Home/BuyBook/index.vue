<template>
    <div>
        <el-button type="primary" @click="showDialog = true">添加购买记录</el-button>
        <el-dialog title="添加购买记录" :visible.sync="showDialog">
            <el-form :model="form">
                <el-form-item label="图书ID">
                    <el-input v-model="form.book_id" type="text" placeholder="图书ID"></el-input>
                </el-form-item>
                <el-form-item label="购买数量">
                    <el-input-number v-model="form.quantity" :min="1" :max="100"></el-input-number>
                </el-form-item>
                <el-form-item label="购买单价">
                    <el-input-number v-model="form.unit_price" :min="0.01" :max="1000" step="0.01"></el-input-number>
                </el-form-item>
            </el-form>
            <span slot="footer" class="dialog-footer">
                <el-button @click="showDialog = false">取消</el-button>
                <el-button type="primary" @click="addBuyBook">确定</el-button>
            </span>
        </el-dialog>
        <el-table :data="buybookList"
                  style="width: 100%"
                  height="450"
                  v-loading.fullscreen.lock="loading"
                  element-loading-text="正在处理..."
                  element-loading-spinner="el-icon-loading"
                  element-loading-background="rgba(0, 0, 0, 0.8)">
            <el-table-column prop="BOOK_NAME" label="书名">
                <template slot-scope="props">
                    <span>《{{ props.row.BOOK_NAME }}》</span>
                </template>
            </el-table-column>
            <el-table-column prop="PURCHASE_DATE" label="购买时间">
                <template slot-scope="props">
                    <span>{{ props.row.PURCHASE_DATE }}</span>
                </template>
            </el-table-column>
            <el-table-column prop="QUANTITY" label="购买数量">
                <template slot-scope="props">
                    <span>{{ props.row.QUANTITY }}</span>
                </template>
            </el-table-column>
            <el-table-column prop="UNIT_PRICE" label="购买单价">
                <template slot-scope="props">
                    <span>{{ props.row.UNIT_PRICE }}</span>
                </template>
            </el-table-column>
            <el-table-column prop="TOTAL_PRICE" label="购买总价">
                <template slot-scope="props">
                    <span>{{ props.row.TOTAL_PRICE }}</span>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>

<script>
    import { mapState } from "vuex";
    import { addBuyBook } from "@/api";
    export default {
        data() {
            return {
                showDialog: false,
                loading: false,
                form: {
                    book_id: "",
                    quantity: "",
                    unit_price: "",
                },
                supplierId: "",
            };
        },
        name: "BuyBook",
        mounted() {
            this.supplierId = this.$route.query.SUPPLIER_ID;
            this.$store.dispatch("initBuyBookList", this.supplierId);
        },
        methods: {
            addBuyBook() {
                if (!this.form.book_id) {
                    this.$message({
                        showClose: true,
                        message: "书籍ID不能为空",
                        type: "error",
                    });
                    return;
                } else if (isNaN(this.form.book_id)) {
                    this.$message({
                        showClose: true,
                        message: "书籍ID必须为数字",
                        type: "error",
                    });
                    return;
                } 
                let data = {
                    admin_id: this.adminInfo.admin_id,
                    supplier_id: this.supplierId,
                    book_id: this.form.book_id,
                    now_time: this.$moment().format("YYYY-MM-DD HH:mm:ss"),
                    quantity: this.form.quantity,
                    unit_price: this.form.unit_price,
                };
                addBuyBook(data).then(
                    (res) => {
                        console.log(res);
                        if (res.status == 200) {
                            this.loading = false;
                            this.$message({
                                showClose: true,
                                message: "添加成功！",
                                type: "success",
                            });
                            this.form.book_id = "";
                            this.form.quantity = "";
                            this.form.unit_price = "";
                            this.showDialog = false;
                            this.$store.dispatch("initBuyBookList", this.supplierId);
                        } else {
                            this.loading = false;
                            this.form.book_id = "";
                            this.form.quantity = "";
                            this.form.unit_price = "";
                            this.showDialog = false;
                            this.$message({
                                showClose: true,
                                message: res.msg,
                                type: "error",
                            });
                        }
                    },
                    (err) => {
                        console.log(err.message);
                        this.loading = false;
                        this.$message({
                            showClose: true,
                            message: "添加失败！",
                            type: "error",
                        });
                        this.form.book_id = "";
                        this.form.quantity = "";
                        this.form.unit_price = "";
                    }
                );
            },
        },
        computed: {
            ...mapState({
                buybookList(state) {
                    return state.BuyBooks.buybookList;
                },
                adminInfo(state) {
                    return state.User.adminInfo;
                },
            }),
        },
    };
</script>

<style lang="less" scoped></style>