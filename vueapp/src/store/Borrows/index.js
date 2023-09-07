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
    initBorrows({ commit }, readerObj) {
        console.log(readerObj);
        let newObj = readerObj
        initBorrows(newObj).then(res=>{
            console.log(res);
            commit('INITBORROWS', res.borrows)
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
        data = data || [];
        state.borrows = data;
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