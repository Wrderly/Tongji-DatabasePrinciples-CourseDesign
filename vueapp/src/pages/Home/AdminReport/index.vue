<template>
    <div>
        <h2>反馈列表</h2>
        <el-table :data="reportList" style="width: 100%" height="450">
            <el-table-column type="expand">
                <template slot-scope="props">
                    <el-form label-position="left" class="demo-table-expand">
                        <el-form-item label="反馈内容：">
                            <span>{{ props.row.FEEDBACK }}</span>
                        </el-form-item>
                    </el-form>
                </template>
            </el-table-column>
            <el-table-column prop="READER_ID" label="用户ID">
                <template slot-scope="props">
                    <span>用户id: {{ props.row.READER_ID }}</span>
                </template>
            </el-table-column>
            <el-table-column prop="REPORT_DATE" label="反馈时间">
                <template slot-scope="props">
                    <span>{{ props.row.REPORT_DATE }}</span>
                </template>
            </el-table-column>
        </el-table>
    </div>
</template>

<script>
    import { mapState } from "vuex";
    export default {
        name: "AdminReport",
        computed: {
            ...mapState({
                reportList(state) {
                    return state.Report.reportList;
                },
            }),
        },
        mounted() {
            this.$store.dispatch('initReportList');
        },
    };
</script>

<style lang="less" scoped></style>
