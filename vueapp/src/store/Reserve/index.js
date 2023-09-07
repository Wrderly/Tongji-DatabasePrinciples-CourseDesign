import {initReservelist,initReserve} from '@/api'

const state = {
   reserveList:[],
   reserve:[],
}

const actions = {
    initReserveList({commit}){
        initReservelist().then(res=>{
            console.log(res);
            
        commit('INITRESERVELIST',res.data)
        },err=>console.log(err.message))
    },
    initReserve({commit},readerObj){
        console.log(readerObj);
        let newObj = readerObj
        initReserve(newObj).then(res=>{
            console.log(res);
        commit('INITRESERVE',res.reserves)
        },err=>{
            console.log(err.message);
        })
    },

}

const mutations = {
    INITRESERVELIST(state,data){
        // 管理员保存预订图书记录
        state.reserveList = data
    },
    INITRESERVE(state, data) {
        data = data || [];
        // 读者保存预订图书记录
        state.reserve = data; 
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