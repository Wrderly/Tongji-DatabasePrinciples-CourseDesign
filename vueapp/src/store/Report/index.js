import {initReportList} from '@/api'
const state = {
  reportList:[]
}

const actions = {
    initReportList({commit}){
        initReportList().then(res=>{
            commit('INITREPORTLIST',res.reports)
        },err=>{
            console.log(err.message);
        })

    }
}

const mutations = {
    INITREPORTLIST(state, data) {
        data = data || [];
        state.reportList = data;
    },
}

const getters = {

}

export default {
    state,
    actions,
    mutations,
    getters
}