<template>
    <div>
        <el-button type="primary" @click="showDialog1 = true" v-if="isAdmin">添加书籍类别</el-button>
        <el-button type="success" @click="showDialog2 = true" v-if="isAdmin">添加书籍</el-button>
        <el-dialog title="添加书籍类别" :visible.sync="showDialog1">
            <el-form :model="form1">
                <el-form-item label="书籍类别">
                    <el-input v-model="form1.collection_type" type="text" placeholder="Booktype"></el-input>
                </el-form-item>
                <el-form-item label="书籍类别注释">
                    <el-input v-model="form1.note" type="text" placeholder="BooktypeNote"></el-input>
                </el-form-item>
            </el-form>
            <el-table :data="booktypeList"
                      height="400"
                      style="width: 100%">
                <el-table-column label="所有书籍类别" prop="collection_type">
                    <template slot-scope="props">
                        <span>{{ props.row.COLLECTION_TYPE }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="所有书籍类别注释" prop="note">
                    <template slot-scope="props">
                        <span>{{ props.row.NOTE }}</span>
                    </template>
                </el-table-column>
            </el-table>
            <span slot="footer" class="dialog-footer">
                <el-button @click="showDialog1 = false">取消</el-button>
                <el-button type="primary" @click="addBooktypes">确定</el-button>
            </span>
        </el-dialog>
        <el-dialog title="添加书籍" :visible.sync="showDialog2">
            <el-form :model="form2">
                <el-form-item label="书籍名称">
                    <el-input v-model="form2.book_name" type="text" placeholder="BookName"></el-input>
                </el-form-item>
                <el-form-item label="书籍作者">
                    <el-input v-model="form2.author" type="text" placeholder="Author"></el-input>
                </el-form-item>
                <el-form-item label="书籍ISBN">
                    <el-input v-model="form2.isbn" type="text" placeholder="ISBN"></el-input>
                </el-form-item>
                <el-form-item label="书籍类别">
                    <br>
                    <el-select v-model="form2.collection_type">
                        <el-option v-for="booktype in booktypeList"
                                   :key="booktype.COLLECTION_TYPE"
                                   :label="booktype.COLLECTION_TYPE"
                                   :value="booktype.COLLECTION_TYPE">
                        </el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="书籍介绍">
                    <el-input v-model="form2.introduction" type="text" placeholder="Introduction"></el-input>
                </el-form-item>
            </el-form>            
            <span slot="footer" class="dialog-footer">
                <el-button @click="showDialog2 = false">取消</el-button>
                <el-button type="primary" @click="addBook">确定</el-button>
            </span>
        </el-dialog>
        <el-input placeholder="请输入您要搜索的图书ID/书名/作者/ISBN/类别"
                  prefix-icon="el-icon-search"
                  @keyup.enter.native="searchBook"
                  @blur="clear"
                  v-model="name">
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
                        </el-form-item>
                    </el-form>
                </template>
            </el-table-column>
            <el-table-column sortable label="图书ID" prop="book_id">
                <template slot-scope="props">
                    <span>{{ props.row.BOOK_ID }}</span>
                </template>
            </el-table-column>
            <el-table-column sortable label="图书名称" prop="book_name">
                <template slot-scope="props">
                    <span>《{{ props.row.BOOK_NAME }}》</span>
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
            <el-table-column label="操作" v-if="isAdmin">
                <template slot-scope="scope">
                    <el-button size="mini"
                               type="primary"
                               plain
                               @click="showDialog_change(scope.$index)">修改</el-button>
                    <el-dialog title="修改书籍" :visible.sync="dialogVisible">
                        <el-form :model="form3">
                            <el-form-item label="书籍名称">
                                <el-input v-model="form3.book_name" type="text"></el-input>
                            </el-form-item>
                            <el-form-item label="书籍作者">
                                <el-input v-model="form3.author" type="text"></el-input>
                            </el-form-item>
                            <el-form-item label="书籍ISBN">
                                <el-input v-model="form3.isbn" type="text"></el-input>
                            </el-form-item>
                            <el-form-item label="书籍类别">
                                <br>
                                <el-select v-model="form3.collection_type">
                                    <el-option v-for="booktype in booktypeList"
                                               :key="booktype.COLLECTION_TYPE"
                                               :label="booktype.COLLECTION_TYPE"
                                               :value="booktype.COLLECTION_TYPE">
                                    </el-option>
                                </el-select>
                            </el-form-item>
                            <el-form-item label="书籍介绍">
                                <el-input v-model="form3.introduction" type="text"></el-input>
                            </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                            <el-button @click="dialogVisible = false">取消</el-button>
                            <el-button type="primary" @click="ChangeBookInfo">确定</el-button>
                        </span>
                    </el-dialog>
                </template>
            </el-table-column>
            <el-table-column label="评论">
                <template slot-scope="scope">
                    <el-button size="mini"
                               type="success"
                               plain
                               @click="bookComments(scope.$index, scope.row)">评论</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>

<script>
    import { mapState } from "vuex";
    import {
        addReserve,
        searchBook,
        AddBooktype,
        AddBook,
        changebookInfo,
    } from "@/api";
    export default {
        name: "SearchBooks",
        data() {
            return {
                loading: false,
                name: "",
                flag: 0,
                searchBooks: [],
                showDialog1: false,
                showDialog2: false,
                dialogVisible: false,
                form1: {
                    collection_type: "",
                    note: ""
                },
                form2: {
                    book_name: "",
                    author: "",
                    isbn: "",
                    collection_type: "",
                    introduction: ""
                },
                form3: {
                    book_name: "",
                    author: "",
                    isbn: "",
                    collection_type: "",
                    introduction: ""
                },
                bookId: "",
            };
        },
        methods: {
            showDialog_change(index) {
                this.bookId = this.booksList[index].BOOK_ID;
                this.form3.book_name = this.booksList[index].BOOK_NAME;
                this.form3.author = this.booksList[index].AUTHOR;
                this.form3.isbn = this.booksList[index].ISBN;
                this.form3.collection_type = this.booksList[index].COLLECTION_TYPE;
                this.form3.introduction = this.booksList[index].INTRODUCTION;
                this.dialogVisible = true;
            },
            addBooktypes(){
                if(!this.form1.collection_type){
                    this.$message({
                                showClose: true,
                                message: "请输入图书类型",
                                type: "error",
                            });
                            return;
                } else if(!this.form1.note){
                    this.$message({
                                showClose: true,
                                message: "请输入图书类型注释",
                                type: "error",
                            });
                            return;
                }
                let data = {
                    collection_type: this.form1.collection_type,
                    note: this.form1.note,
                };
                AddBooktype(data).then(
                    (res) => {
                        this.loading = false;
                        console.log(res);
                        if (res.status == 200) {
                            this.$message({
                                showClose: true,
                                message: "添加成功",
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
                        this.form1.collection_type = "";
                        this.form1.note = "";
                        this.$store.dispatch("initBooktypeList");
                    },
                    (err) => {
                        this.loading = false;
                        console.log(err.message);
                    }
                );
            },
            addBook() {
                if (!this.form2.book_name) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书名称",
                        type: "error",
                    });
                    return;
                } else if (!this.form2.author) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书作者",
                        type: "error",
                    });
                    return;
                } else if (!this.form2.isbn) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书ISBN",
                        type: "error",
                    });
                    return;
                } else if (!this.form2.introduction) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书介绍",
                        type: "error",
                    });
                    return;
                } else {
                    var regex = /^(97(8|9))?\d{9}(\d|X)$/;
                    if (!regex.test(this.form2.isbn)) {
                        this.$message({
                            showClose: true,
                            message: "请输入有效的图书ISBN",
                            type: "error",
                        });
                        return;
                    }
                    let data = {
                        book_name: this.form2.book_name,
                        author: this.form2.author,
                        ISBN: this.form2.isbn,
                        collection_type: this.form2.collection_type,
                        introduction: this.form2.introduction,
                    };
                    AddBook(data).then(
                        (res) => {
                            this.loading = false;
                            console.log(res);
                            if (res.status == 200) {
                                this.$message({
                                    showClose: true,
                                    message: "添加成功",
                                    type: "success",
                                });
                                this.showDialog2 = false;
                                this.form2.book_name = "";
                                this.form2.author = "";
                                this.form2.collection_type = "";
                                this.form2.isbn = "";
                                this.form2.introduction = "";
                                this.$store.dispatch("initBooksList");
                            }
                            else {
                                this.$message({
                                    showClose: true,
                                    message: res.msg,
                                    type: "error",
                                });
                            }
                        },
                        (err) => {
                            this.loading = false;
                            console.log(err.message);
                        }
                    );
                }
            },
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
                        if (res.status == 200) {
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
                    searchStr: this.name,
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
            bookComments(index, row) {
                this.$router.push({ path: "/home/comment", query: { BOOK_ID: row.BOOK_ID } });
            },
            clear() {
                this.flag = 0;
                this.searchBooks = [];
            },
            ChangeBookInfo() {
                if (!this.form3.book_name) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书名称",
                        type: "error",
                    });
                    return;
                } else if (!this.form3.author) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书作者",
                        type: "error",
                    });
                    return;
                } else if (!this.form3.isbn) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书ISBN",
                        type: "error",
                    });
                    return;
                } else if (!this.form3.introduction) {
                    this.$message({
                        showClose: true,
                        message: "请输入图书介绍",
                        type: "error",
                    });
                    return;
                } else {
                    var regex = /^(97(8|9))?\d{9}(\d|X)$/;
                    if (!regex.test(this.form3.isbn)) {
                        this.$message({
                            showClose: true,
                            message: "请输入有效的图书ISBN",
                            type: "error",
                        });
                        return;
                    }
                }
                let data = {
                    book_id: this.bookId,
                    book_name: this.form3.book_name,
                    author: this.form3.author,
                    ISBN: this.form3.isbn,
                    collection_type: this.form3.collection_type,
                    introduction: this.form3.introduction,
                };
                changebookInfo(data).then(
                    (res) => {
                        this.loading = false;
                        console.log(res);
                        if (res.status == 200) {
                            this.$message({
                                showClose: true,
                                message: "修改成功",
                                type: "success",
                            });
                            this.dialogVisible = false;
                            this.bookId = "";
                            this.form3.book_name = "";
                            this.form3.author = "";
                            this.form3.collection_type = "";
                            this.form3.isbn = "";
                            this.form3.introduction = "";
                            this.$store.dispatch("initBooksList");
                        }
                        else {
                            this.$message({
                                showClose: true,
                                message: res.msg,
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
                booktypeList(state) {
                    return state.BookTypes.booktypeList;
                },
            }),
        },
        mounted() {
            this.$store.dispatch("initBooksList");
            this.$store.dispatch("initBooktypeList");
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
    .el-input {
        margin-top: 20px;
    }
</style>
