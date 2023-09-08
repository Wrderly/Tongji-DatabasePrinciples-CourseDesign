import { initSupplierList } from "@/api";

const state = {
    supplierList: [],
};

const actions = {
    initSupplierList({ commit }) {
        initSupplierList().then(
            (res) => {
                console.log(res);
                commit("INITSUPPLIERLIST", res.suppliers);
            },
            (err) => console.log(err.message)
        );
    },
};

const mutations = {
    INITSUPPLIERLIST(state, data) {
        data = data || [];
        state.supplierList = data;
    },
};

const getters = {};

export default {
    state,
    actions,
    mutations,
    getters,
};