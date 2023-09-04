import {initReservelist,initReserve} from '@/api'
import qs from "qs" 

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
        console.log(qs.stringify(readerObj));
        let newObj = qs.stringify(readerObj)
        initReserve(newObj).then(res=>{
            console.log(res);
        commit('INITRESERVE',res.data)
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
    INITRESERVE(state,data){
        // 读者保存预订图书记录
        state.reserve = data||[]
        state.reserve.forEach((element,index)=>{
            element.bookName = '《'+element.bookName+'》'
        }
            
        )
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