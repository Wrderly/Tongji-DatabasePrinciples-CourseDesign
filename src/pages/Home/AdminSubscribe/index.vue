<template>
   <el-table
    :data="reserveList"
    style="width: 100%"
    height="450"
    :default-sort = "{prop: 'date', order: 'descending'}"
    >
    <el-table-column type="expand">
      <template slot-scope="props">
        <el-form label-position="left" class="demo-table-expand">
          <el-form-item label="预约日期：">
            <span>{{ props.row.date }}</span>
          </el-form-item>
          <el-form-item label="图书 ID：">&nbsp;
            <span>{{ props.row.bookId }}</span>
          </el-form-item>
          <el-form-item label="图书名称：">
            <span>{{ props.row.bookName }}</span>
          </el-form-item>
          <el-form-item label="读者编号：">
            <span>{{ props.row.readerId }}</span>
          </el-form-item>
          <el-form-item label="读者姓名：">
            <span>{{ props.row.readerName }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column
      label="预约日期"
      sortable
      prop="date"
      >
      <template slot-scope="scope">
        <i class="el-icon-time"></i>
        <span style="margin-left: 10px">{{ scope.row.date }}</span>
      </template>
    </el-table-column>
    <el-table-column
      label="图书名称"
      sortable
      prop="bookName"
      >
      <template slot-scope="scope">
        <el-popover trigger="hover" placement="top">
          <p>编号: {{ scope.row.bookId }}</p>
          <p>名称: {{ scope.row.bookName }}</p>
          <div slot="reference" class="name-wrapper">
            <el-tag size="medium">{{ scope.row.bookName }}</el-tag>
          </div>
        </el-popover>
      </template>
    </el-table-column>
    <el-table-column
      label="读者姓名"
      sortable
      prop="readerName"
      >
      <template slot-scope="scope">
        <el-popover trigger="hover" placement="top">
          <p>编号: {{ scope.row.readerId }}</p>
          <p>姓名: {{ scope.row.readerName }}</p>
          <div slot="reference" class="name-wrapper">
            <el-tag size="medium">{{ scope.row.readerName }}</el-tag>
          </div>
        </el-popover>
      </template>
    </el-table-column>
    <el-table-column label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="danger"
          @click="handleDelete(scope.$index, scope.row)">删除记录</el-button>
      </template>
    </el-table-column>
  </el-table>
</template>

<script>
import {mapState} from 'vuex'
import {deleteReserve,initReserve} from '@/api'
import qs from 'qs'
export default {
    name: 'AdminSubscribe',
    data() {
        return {
            
        };
    },
    computed:{
        ...mapState({
            reserveList(state){
                return state.Reserve.reserveList
            }
        })
    },
    mounted() {
        this.$store.dispatch('initReserveList')
    },

    methods: {
        handleDelete(index,row){
            console.log(index,row);
            let reserveObj = {readerId:row.readerId,bookId:row.bookId,date:row.date}
            deleteReserve(qs.stringify(reserveObj)).then(res=>{
                console.log(res);
                this.$store.dispatch('initReserveList')
                
            },err=>{
                console.log(err.message);
                
            })
        }
    },
};
</script>

<style lang="less" scoped>

</style>