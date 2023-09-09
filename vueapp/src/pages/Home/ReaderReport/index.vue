<template>
    <div v-loading.fullscreen.lock="loading"
         element-loading-text="正在处理..."
         element-loading-spinner="el-icon-loading"
         element-loading-background="rgba(0, 0, 0, 0.8)"
         class="clearfix wrap">
        <div>
            <h3>用户反馈</h3>
            <el-input class="textarea"
                      type="textarea"
                      :rows="10"
                      placeholder="请输入内容"
                      v-model="textarea">
            </el-input>
            <el-button type="primary" plain class="sendreport" @click="sendreport">提交反馈</el-button>
        </div>
    </div>
</template>

<script>
    import { mapState } from "vuex";
    import { reader_report } from "@/api";
    export default {
        name: "ReaderReport",
        data() {
            return {
                loading: false,
                textarea: "",
            };
        },
        computed: {
            ...mapState({
                readerInfo(state) {
                    return state.User.readerInfo;
                },
            }),
        },
        methods: {
            sendreport() {
                if (!this.textarea) {
                    this.$message({
                        showClose: true,
                        message: "反馈不能为空",
                        type: "error",
                    });
                    return;
                } else if (this.textarea.length > 900) {
                    this.$message({
                        showClose: true,
                        message: "反馈过长，最多输入300个汉字",
                        type: "error",
                    });
                    return;
                }
                this.loading = true;
                let data = {
                    reader_id: this.readerInfo.reader_id,
                    content: this.textarea,
                    now_time: this.$moment().format("YYYY-MM-DD HH:mm:ss")
                };
                reader_report(data).then(
                    (res) => {
                        console.log(res);
                        if (res.status == 200) {
                            this.loading = false;
                            this.$message({
                                showClose: true,
                                message: "反馈成功！",
                                type: "success",
                            });
                            this.textarea = "";
                        } else {
                            this.loading = false;
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
                            message: "反馈失败！",
                            type: "error",
                        });
                    }
                );
            },
        },
    };
</script>

<style lang="less" scoped>
    .wrap {
        position: relative;
        .textarea

    {
        //position: absolute;
        margin-top: 20px;
    }

    .sendreport {
        margin-top: 20px;
        // position: absolute;
        // right: 0;
        margin-left: 88%;
    }
    }
</style>
