import {initBorrowslist,initBorrows} from '@/api'
// import {initBorrowslist} from '@/api'


const state = {
    // 管理员接收所有记录
  borrowsList:[],
    //读者只接收自己的借阅记录   
  borrows:[]
}

// readerName:'',
// bookName:'',
// date:'',
// content:'',
// prise:0

const actions = {
    initBorrowsList({commit}){
        initBorrowslist().then(res=>{
            console.log(res);
            
        commit('INITBORROWSLIST',res.data)
        },err=>console.log(err.message))
    },
    initBorrows({commit},data){
        console.log('borrow',data);
        initBorrows(data).then(res=>{
            console.log(res);
        commit('INITBORROWS',res.data)
        },err=>{
            console.log(err.message);
        })
    },
}

const mutations = {
    INITBORROWSLIST(state,data){
        // 管理员保存借书记录的数组
        state.borrowsList = data
    },
    INITBORROWS(state,data){
        // 读者保存自己的记录
        state.borrows = data
    }
}

const getters = {

}

export default {
    state,
    actions,
    mutations,
    getters
}