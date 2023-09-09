<template>
    <el-form ref="form" label-width="80px">
        <el-form-item label="书籍名称" label-width="80px">
            <el-col :span="8">
                <el-input v-model="bookName"></el-input>
            </el-col>
        </el-form-item>
        <el-form-item label="书籍作者" label-width="80px">
            <el-col :span="8">
                <el-input v-model="author"></el-input>
            </el-col>
        </el-form-item>
        <el-form-item label="ISBN" label-width="80px">
            <el-col :span="8">
                <el-input v-model="ISBN"></el-input>
            </el-col>
        </el-form-item>
        <el-form-item label="书籍类别" label-width="80px">
            <el-col :span="8">
                <el-input v-model="kind"></el-input>
            </el-col>
        </el-form-item>
        <el-form-item>
            <el-col :span="6">
                <el-button type="primary" @click="addBook">立即添加</el-button>

            </el-col>
        </el-form-item>
    </el-form>
</template>

<script>
import { mapState } from "vuex";
import { addBooks } from '@/api'
import qs from 'qs'
export default {
    name: 'AdminAddBooks',

    data() {
        return {
            bookName: '',
            author: '',
            ISBN: '',
            kind: ''
        }
    },
    computed: {
        ...mapState({
            booksList(state) {
                return state.Books.booksList;
            }

        })
    },
    methods: {
        addBook() {
            let { bookName, author, ISBN, kind } = this
            let infoObj = { bookName, author, ISBN, kind }
            addBooks(qs.stringify(infoObj)).then(res => {
                console.log(res);
                if (res.status == 200) {
                    this.$message({
                        showClose: true,
                        message: res.msg,
                        type: 'success',
                    });
                    this.bookName = '',
                        this.author = '',
                        this.ISBN = '',
                        this.kind = ''
                    this.$store.dispatch('initBooksList')

                } else {
                    this.$message({
                        showClose: true,
                        message: res.msg,
                        type: 'error',
                    });
                }


            }, err => {
                console.log(err.message);

            })
        }
    }

};
</script>

<style lang="less" scoped>
.my-autocomplete {
    li {
        line-height: normal;
        padding: 7px;

        .name {
            text-overflow: ellipsis;
            overflow: hidden;
        }

        .addr {
            font-size: 12px;
            color: #b4b4b4;
        }

        .highlighted .addr {
            color: #ddd;
        }
    }
}
</style>