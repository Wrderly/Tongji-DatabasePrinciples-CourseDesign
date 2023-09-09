<template>
    <div v-loading.fullscreen.lock="loading"
         element-loading-text="正在处理..."
         element-loading-spinner="el-icon-loading"
         element-loading-background="rgba(0, 0, 0, 0.8)"
         class="clearfix wrap">
        <div v-if="commentsList.length === 0">暂无评论</div>
        <div style="height: 400px; overflow-y: auto;">
            <div class="comment"
                 v-for="(comment, index) of commentsList"
                 :key="index">
                <el-popconfirm v-if="comment.READER_ID == readerInfo.reader_id || isAdmin" title="确认删除吗？"
                               @confirm="delComment(comment.REVIEW_ID)">
                    <el-button size="mini" type="danger" class="report" slot="reference">删除</el-button>
                </el-popconfirm>
                <div class="reader">{{ (comment.READER_ID != readerInfo.reader_id || isAdmin) ? '用户 ' + comment.READER_NAME : '你' }}</div>
                <div class="time">{{ comment.REVIEW_DATE }}</div>
                <div class="content">{{ comment.REVIEW_TEXT }}</div>
                <el-divider></el-divider>
            </div>
        </div>
        <div>
            <!--<el-select v-model="bookId" placeholder="请选择书籍">
              <el-option
                v-for="item in booksList"
                :key="item.bookId"
                :label="item.bookName"
                :value="item.bookId"
              >
              </el-option>
            </el-select>-->

            <el-input class="textarea"
                      type="textarea"
                      :rows="2"
                      placeholder="请输入内容"
                      v-model="textarea"
                      v-if="!isAdmin">
            </el-input>
            <el-button type="primary" plain class="sendcomment" @click="sendcomment" v-if="!isAdmin">发表评论</el-button>
        </div>
    </div>
</template>

<script>
    import { mapState } from "vuex";
    import { addComment, auditComment } from "@/api";
    export default {
        name: "CoMMent",

        data() {
            return {
                loading: false,
                textarea: "",
                bookId: "",
            };
        },
        computed: {
            ...mapState({
                commentsList(state) {
                    return state.Comments.commentsList;
                },
                isAdmin(state) {
                    return state.User.isAdmin;
                },
                readerInfo(state) {
                    if (!this.isAdmin)
                        return state.User.readerInfo;
                    else return {};
                },
            }),
        },
        methods: {
            sendcomment() {
                this.loading = true;
                if (!this.textarea) {
                    this.$message({
                        showClose: true,
                        message: "请输入评论内容",
                        type: "error",
                    });
                    return;
                }
                let dataObj = {
                    reader_id: this.readerInfo.reader_id,
                    book_id: this.bookId,
                    review_text: this.textarea,
                    now_time: this.$moment().format("YYYY-MM-DD HH:mm:ss")
                };
                addComment(dataObj).then(
                    (res) => {
                        this.loading = false;
                        if (res.status == 200) {
                            this.$message({
                                showClose: true,
                                message: "评论添加成功",
                                type: "success",
                            });
                            console.log(res);
                            this.$store.dispatch("initCommentsList", this.bookId);
                            this.textarea = "";
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
            delComment(reviewId) {
                this.loading = true;
                let infoObj = {
                    review_id: reviewId.toString()
                };
                auditComment(infoObj).then(
                    (res) => {
                        this.loading = false;
                        if (res.status == 200) {
                            this.$message({
                                showClose: true,
                                message: "删评成功！",
                                type: "success",
                            });
                            console.log(res);
                            this.$store.dispatch("initCommentsList", this.bookId);
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
        mounted() {
            this.bookId = this.$route.query.BOOK_ID;
            this.$store.dispatch("initCommentsList", this.bookId);
        },
    };
</script>

<style lang="less" scoped>
    .wrap {
        position: relative;
        .comment

    {
        position: relative;
        .report

    {
        float: right;
        margin-left: 20px;
        // margin-right: 30%;
    }

    .time {
        font-family: inherit;
        font-style: italic;
        font-size: small;
        color: #79cde2;
        margin: 10px;
    }

    .reader {
        font-style: italic;
        font-family: Arial;
        position: absolute;
        bottom: 5px;
        right: 50px;
    }

    .content {
        text-indent: 2em;
    }

    }

    .textarea {
        // position: absolute;
        margin-top: 20px;
    }

    .sendcomment {
        margin-top: 20px;
        // position: absolute;
        // right: 0;
        margin-left: 88%;
    }
    }
</style>
